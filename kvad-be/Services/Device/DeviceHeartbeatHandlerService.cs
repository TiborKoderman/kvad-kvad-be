using kvad_be.Database;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using System.Text.Json;

public class DeviceHeartbeatHandlerService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DeviceHeartbeatHandlerService> _logger;
    private readonly GroupService _groupService;
    private readonly IClock _clock;
    private readonly TopicHub _topicHub;

    public DeviceHeartbeatHandlerService(
        AppDbContext context,
        ILogger<DeviceHeartbeatHandlerService> logger,
        GroupService groupService,
        IClock clock,
        TopicHub topicHub
    )
    {
        _context = context;
        _logger = logger;
        _groupService = groupService;
        _clock = clock;
        _topicHub = topicHub;
    }

    public async Task HandleHeartbeatAsync(Guid deviceId, HeartbeatDTO heartbeat)
    {
        try
        {
            var device = await _context.Devices
                .Include(d => d.State)
                .Include(d => d.HeartbeatSettings)
                .FirstOrDefaultAsync(d => d.Id == deviceId);

            if (device == null)
            {
                _logger.LogWarning("Device with ID {DeviceId} not found.", deviceId);
                return;
            }

            // Initialize device state if it doesn't exist
            if (device.State == null)
            {
                device.State = new DeviceState { DeviceId = deviceId };
                // Device state will be saved when the device is saved
            }

            var previousHeartbeat = device.State.LastHeartbeat;
            var now = _clock.GetCurrentInstant();

            // Update basic heartbeat information
            device.State.LastHeartbeat = Instant.FromUnixTimeSeconds(heartbeat.Ts);
            device.State.BootId = heartbeat.BootId.ToString();
            device.State.Seq = heartbeat.Seq;
            device.State.UptimeSec = (int)heartbeat.UptimeS;
            device.State.ConfigHash = heartbeat.CfgHash;

            // Update flags and extra data if provided
            if (heartbeat.Flags != null)
            {
                device.State.Flags = JsonDocument.Parse(JsonSerializer.Serialize(heartbeat.Flags));
            }

            if (heartbeat.Extra != null)
            {
                device.State.Extra = JsonDocument.Parse(JsonSerializer.Serialize(heartbeat.Extra));
            }

            // Update connectivity status based on heartbeat timing
            UpdateConnectivityStatus(device, previousHeartbeat, now);

            // Update health status based on flags or other indicators
            UpdateHealthStatus(device, heartbeat);

            // Detect device reboot
            DetectDeviceReboot(device, heartbeat);

            await _context.SaveChangesAsync();

            await _topicHub.PublishJsonAsync(
                topic: $"device/state",
                payload: device.State
                );
            _logger.LogDebug(
                "Heartbeat processed for device {DeviceId}. Seq: {Seq}, Uptime: {Uptime}s, Connectivity: {Connectivity}",
                deviceId, heartbeat.Seq, heartbeat.UptimeS, device.State.Connectivity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing heartbeat for device {DeviceId}", deviceId);
            throw;
        }
    }

    private void UpdateConnectivityStatus(Device device, Instant? previousHeartbeat, Instant now)
    {
        var heartbeatSettings = device.HeartbeatSettings;
        if (heartbeatSettings == null)
        {
            // Use default settings if not configured
            device.State!.Connectivity = DeviceConnectivity.Online;
            return;
        }

        if (previousHeartbeat.HasValue)
        {
            var timeSinceLastHeartbeat = now - previousHeartbeat.Value;
            var expectedInterval = Duration.FromSeconds(heartbeatSettings.ExpectedInterval);
            var threshold = Duration.FromSeconds(heartbeatSettings.HbIntervalThreshold);

            if (timeSinceLastHeartbeat <= expectedInterval * 1.2) // Within 20% of expected interval
            {
                device.State!.Connectivity = DeviceConnectivity.Online;
            }
            else if (timeSinceLastHeartbeat <= threshold)
            {
                device.State!.Connectivity = DeviceConnectivity.Intermittent;
            }
            else
            {
                device.State!.Connectivity = DeviceConnectivity.Offline;
            }
        }
        else
        {
            // First heartbeat received
            device.State!.Connectivity = DeviceConnectivity.Online;
        }
    }

    private void UpdateHealthStatus(Device device, HeartbeatDTO heartbeat)
    {
        // Default to healthy if we're receiving heartbeats
        device.State!.Health = DeviceHealth.Healthy;

        // Check for warning/critical flags in the heartbeat
        if (heartbeat.Flags != null)
        {
            var flags = heartbeat.Flags.Where(f => f != null).ToList();

            if (flags.Any(f => f!.Contains("critical", StringComparison.OrdinalIgnoreCase) ||
                              f.Contains("error", StringComparison.OrdinalIgnoreCase)))
            {
                device.State.Health = DeviceHealth.Critical;
            }
            else if (flags.Any(f => f!.Contains("warning", StringComparison.OrdinalIgnoreCase) ||
                                   f.Contains("throttled", StringComparison.OrdinalIgnoreCase)))
            {
                device.State.Health = DeviceHealth.Warning;
            }
        }
    }

    private void DetectDeviceReboot(Device device, HeartbeatDTO heartbeat)
    {
        // Check if device has rebooted (sequence number reset or new boot ID)
        if (device.State!.Seq > heartbeat.Seq && device.State.BootId != heartbeat.BootId.ToString())
        {
            _logger.LogInformation(
                "Device {DeviceId} appears to have rebooted. Previous seq: {PrevSeq}, New seq: {NewSeq}, Boot ID: {BootId}",
                device.Id, device.State.Seq, heartbeat.Seq, heartbeat.BootId);

            // Reset connectivity to online for fresh start
            device.State.Connectivity = DeviceConnectivity.Online;
        }
    }

    public async Task CheckStaleDevicesAsync()
    {
        try
        {
            var now = _clock.GetCurrentInstant();
            var staleThreshold = now - Duration.FromMinutes(5); // Consider devices stale if no heartbeat for 5 minutes

            var staleDevices = await _context.Devices
                .Include(d => d.State)
                .Include(d => d.HeartbeatSettings)
                .Where(d => d.State != null &&
                           d.State.LastHeartbeat.HasValue &&
                           d.State.LastHeartbeat < staleThreshold &&
                           d.State.Connectivity != DeviceConnectivity.Offline)
                .ToListAsync();

            foreach (var device in staleDevices)
            {
                var timeSinceLastHeartbeat = now - device.State!.LastHeartbeat!.Value;
                var expectedInterval = Duration.FromSeconds(device.HeartbeatSettings?.ExpectedInterval ?? 30);

                if (timeSinceLastHeartbeat > expectedInterval * 3)
                {
                    device.State.Connectivity = DeviceConnectivity.Offline;
                    device.State.Health = DeviceHealth.Unknown;

                    _logger.LogWarning(
                        "Device {DeviceId} marked as offline. Last heartbeat: {LastHeartbeat}",
                        device.Id, device.State.LastHeartbeat);
                }
            }

            if (staleDevices.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for stale devices");
        }
    }
}