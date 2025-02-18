using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.Server;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class MqttBackgroundService : BackgroundService
{
    private readonly ILogger<MqttBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private MqttServer? _mqttServer;

    public MqttBackgroundService(IServiceProvider serviceProvider, ILogger<MqttBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting MQTT server...");

        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .Build();

        _mqttServer = new MqttServer(mqttServerOptions);

        using var scope = _serviceProvider.CreateScope();
        var mqttController = scope.ServiceProvider.GetRequiredService<MqttController>();

        // Attach event handlers
        _mqttServer.ValidatingConnectionAsync += mqttController.ValidateConnection;
        _mqttServer.ClientConnectedAsync += mqttController.OnClientConnected;

        // Start the MQTT server
        await _mqttServer.StartAsync();

        _logger.LogInformation("MQTT server started.");

        // Keep service running until stopped
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping MQTT server...");
        if (_mqttServer != null)
        {
            await _mqttServer.StopAsync();
            _mqttServer.Dispose();
        }
        _logger.LogInformation("MQTT server stopped.");
    }
}
