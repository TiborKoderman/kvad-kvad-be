using Microsoft.EntityFrameworkCore;

public class DeviceService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DeviceService> _logger;

    private readonly GroupService _groupService;

    public DeviceService(AppDbContext context, ILogger<DeviceService> logger, GroupService groupService)
    {
        _context = context;
        _logger = logger;
        _groupService = groupService;
    }

    public async Task<List<Device>> GetAllDevices()
    {
        return await _context.Devices.ToListAsync();
    }

    public async Task<List<Device>> GetAllDevicesOfUser(User user)
    {
        return await _context.Devices
            .Where(d => d.Groups.Any(g => g.Users.Any(u => u.Id == user.Id)))
            .ToListAsync();
    }

    public async Task<List<Device>> GetAllDevicesOfGroup(Group group)
    {
        return await _context.Devices
            .Where(d => d.Groups.Any(g => g.Id == group.Id))
            .ToListAsync();
    }

    public async Task<Device?> GetDeviceById(Guid id)
    {
        return await _context.Devices.FindAsync(id);
    }

    public async Task AddDevice(Device device)
    {
        await _context.Devices.AddAsync(device);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateDevice(Device device)
    {
        _context.Devices.Update(device);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateDevice(DeviceDTO deviceDTO)
    {
        var device = await GetDeviceById(deviceDTO.Id);
        if (device != null)
        {
            device.Name = deviceDTO.Name;
            device.Description = deviceDTO.Description ?? "";
            device.Location = deviceDTO.Location ?? "";
            device.Type = deviceDTO.Type;
            device.Virtual = deviceDTO.Virtual;
            device.State = deviceDTO.State;

            await UpdateDevice(device);
        }
    }

    public async Task DeleteDevice(Guid id)
    {
        var device = await GetDeviceById(id);
        if (device != null)
        {
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<TagSource>> GetAllTagSources()
    {
        return await _context.TagSources.ToListAsync();
    }

    public async Task<List<HistoricizationInterval>> GetHistoricizationIntervals()
    {
        return await _context.HistoricizationIntervals.ToListAsync();
    }

    public async Task<List<UnitDTO>> GetUnits()
    {
        return await _context.Units.Select(u => new UnitDTO(u.Id, u.Symbol)).ToListAsync();
    }
}