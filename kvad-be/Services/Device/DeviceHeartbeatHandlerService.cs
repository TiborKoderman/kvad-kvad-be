using kvad_be.Database;
public class DeviceHeartbeatHandlerService
{
  private readonly AppDbContext _context;
  private readonly ILogger<DeviceHeartbeatHandlerService> _logger;

  private readonly GroupService _groupService;

  public DeviceHeartbeatHandlerService(AppDbContext context, ILogger<DeviceHeartbeatHandlerService> logger, GroupService groupService)
  {
    _context = context;
    _logger = logger;
    _groupService = groupService;
  }
  public Task HandleHeartbeatAsync(Guid deviceId, HeartbeatDTO heartbeat)
  {
    var deviceState = _context.Devices.FirstOrDefault(d => d.Id == deviceId)?.State;

    if (deviceState == null)
    {
      _logger.LogWarning("Device with ID {DeviceId} not found.", deviceId);
      return Task.CompletedTask;
    }


    deviceState.LastHeartbeat = heartbeat.Ts;
    deviceState.BootId = heartbeat.BootId.ToString();
    deviceState.Seq = heartbeat.Seq;
    deviceState.UptimeSec = (int)heartbeat.UptimeS;


    // Implement heartbeat handling logic here
    return Task.CompletedTask;
  }
}