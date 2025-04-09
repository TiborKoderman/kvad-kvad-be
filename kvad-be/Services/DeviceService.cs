using Microsoft.EntityFrameworkCore;

public class DeviceService {
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
            .Where(d => _groupService.IsMemberOfAnyGroup(user, d.Groups))
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

    public async Task DeleteDevice(Guid id)
    {
        var device = await GetDeviceById(id);
        if (device != null)
        {
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
        }
    }
}