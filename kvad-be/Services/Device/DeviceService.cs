using Microsoft.EntityFrameworkCore;
using kvad_be.Database;
using NodaTime;
using System.Globalization;

public class DeviceService(AppDbContext context, ILogger<DeviceService> logger, GroupService groupService)
{
    public async Task<List<Device>> GetAllDevices()
    {
        return await context.Devices.ToListAsync();
    }

    public async Task<List<DeviceDTO>> GetAllDevicesOfUser(User user)
    {
        return await context.Devices
            .Include(d => d.Tags)
            .Include(d => d.State)
            .Where(d => d.Groups.Any(g => g.Users.Any(u => u.Id == user.Id)))
            .Select(d => new DeviceDTO(
                d.Id,
                d.Name,
                d.Description,
                d.OwnerId,
                d.Location,
                d.Type,
                d.Virtual,
                d.Tags.Select(t => new TagDTO(
                    t.Id.ToString(CultureInfo.InvariantCulture),
                    t.Path,
                    null, // Description
                    t.UnitId,
                    "", // Expression - empty for now
                    t.Enabled,
                    t.HistPolicies.Count != 0, // Historicize
                    "Device" // Source
                )).ToList(),
                d.State != null ? new DeviceStateDTO(
                    d.State.DeviceId,
                    d.State.Connectivity,
                    d.State.Health,
                    d.State.Mode,
                    d.State.LastHeartbeat
                ) : null
            ))
            .ToListAsync().ConfigureAwait(false);
    }


    public async Task<Device?> GetDeviceById(Guid id)
    {
        return await context.Devices.FindAsync(id);
    }

    public async Task AddDevice(Device device)
    {
        await context.Devices.AddAsync(device);
        await context.SaveChangesAsync();
    }

    public async Task UpdateDevice(Device device)
    {
        context.Devices.Update(device);

        await context.SaveChangesAsync();
    }

    public async Task UpdateDevice(DeviceDTO deviceDTO)
    {
        var device = await GetDeviceById(deviceDTO.Id);
        if (device != null)
        {
            device.Name = deviceDTO.Name;
            device.Description = deviceDTO.Description ?? string.Empty;
            device.Location = deviceDTO.Location ?? string.Empty;
            device.Type = deviceDTO.Type ?? string.Empty;
            device.Virtual = deviceDTO.Virtual;
            await UpdateDevice(device);
        }
    }

    public async Task DeleteDevice(Guid id)
    {
        var device = await GetDeviceById(id);
        if (device != null)
        {
            context.Devices.Remove(device);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<TagSource>> GetAllTagSources()
    {
        return await context.TagSources.ToListAsync();
    }

    public async Task ProcessHeartbeatAsync(Guid deviceId, HeartbeatDTO heartbeat)
    {
        var device = await context.Devices
            .Include(d => d.State)
            .FirstOrDefaultAsync(d => d.Id == deviceId);

        if (device == null)
        {
            logger.LogWarning("Device with ID {DeviceId} not found for heartbeat processing.", deviceId);
            return;
        }

        // Initialize device state if it doesn't exist
        if (device.State == null)
        {
            device.State = new DeviceState { DeviceId = deviceId };
        }

        // Update heartbeat information
        device.State.LastHeartbeat = NodaTime.Instant.FromUnixTimeSeconds(heartbeat.Ts);
        device.State.BootId = heartbeat.BootId.ToString();
        device.State.Seq = heartbeat.Seq;
        device.State.UptimeSec = (int)heartbeat.UptimeS;
        device.State.ConfigHash = heartbeat.CfgHash;

        // Update connectivity to Online when receiving heartbeat
        device.State.Connectivity = DeviceConnectivity.Online;
        device.State.Health = DeviceHealth.Healthy;

        await context.SaveChangesAsync();

        logger.LogDebug("Heartbeat processed for device {DeviceId}", deviceId);
    }

    public async Task<List<Device>> GetDevicesWithStaleHeartbeats(TimeSpan staleThreshold)
    {
        var cutoffTime = SystemClock.Instance.GetCurrentInstant() - Duration.FromTimeSpan(staleThreshold);

        return await context.Devices
            .Include(d => d.State)
            .Where(d => d.State != null &&
                       d.State.LastHeartbeat.HasValue &&
                       d.State.LastHeartbeat < cutoffTime)
            .ToListAsync();
    }

}