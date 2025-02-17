using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


public class MqttBrokerService : BackgroundService
{
    private readonly ILogger<MqttBrokerService> _logger;
    private Mqtt _mqttServer;

    public MqttBrokerService(ILogger<MqttBrokerService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting MQTT Broker Service...");

        // Build the MQTT server options.
        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()             // Listen on all network interfaces.
            .WithDefaultEndpointPort(1883)       // Default MQTT port.
            .Build();

        // Create the MQTT server instance.
        _mqttServer = new MqttFactory().CreateMqttServer();

        // Attach event handlers.
        _mqttServer.ClientConnectedAsync += e =>
        {
            _logger.LogInformation("Client connected: {ClientId}", e.ClientId);
            return Task.CompletedTask;
        };

        _mqttServer.ClientDisconnectedAsync += e =>
        {
            _logger.LogInformation("Client disconnected: {ClientId}", e.ClientId);
            return Task.CompletedTask;
        };

        _mqttServer.ApplicationMessageReceivedAsync += e =>
        {
            string payload = e.ApplicationMessage.Payload is null
                ? string.Empty
                : Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            _logger.LogInformation("Received message from client {ClientId} on topic {Topic}: {Payload}",
                e.ClientId, e.ApplicationMessage.Topic, payload);
            return Task.CompletedTask;
        };

        // Start the MQTT broker.
        await _mqttServer.StartAsync(options);
        _logger.LogInformation("MQTT Broker started.");

        // Wait here until the service is cancelled.
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            // Expected when the service is stopping.
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping MQTT Broker Service...");
        if (_mqttServer != null)
        {
            await _mqttServer.StopAsync();
        }
        await base.StopAsync(cancellationToken);
    }
}