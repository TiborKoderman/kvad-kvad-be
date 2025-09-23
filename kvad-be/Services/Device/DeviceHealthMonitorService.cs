

public class DeviceHealthMonitorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeviceHealthMonitorService> _logger;
    private readonly TimeSpan _checkInterval;

    public DeviceHealthMonitorService(
        IServiceProvider serviceProvider,
        ILogger<DeviceHealthMonitorService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        // Configure check interval from configuration (default: 1 minute)
        var intervalMinutes = configuration.GetValue<int>("DeviceHealthMonitor:CheckIntervalMinutes", 1);
        _checkInterval = TimeSpan.FromMinutes(intervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Device Health Monitor Service started. Check interval: {Interval}", _checkInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckDeviceHealth();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking device health");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckDeviceHealth()
    {
        using var scope = _serviceProvider.CreateScope();
        var heartbeatHandler = scope.ServiceProvider.GetRequiredService<DeviceHeartbeatHandlerService>();

        await heartbeatHandler.CheckStaleDevicesAsync();
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Device Health Monitor Service is stopping.");
        await base.StopAsync(stoppingToken);
    }
}