using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


public class MqttBrokerService
{
    private readonly ILogger<MqttBrokerService> _logger;
    private readonly IMqttClient _mqttClient;

    public MqttBrokerService(ILogger<MqttBrokerService> logger, IMqttClient mqttClient)
    {
        _logger = logger;
        _mqttClient = mqttClient;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MqttBrokerService is starting.");

        _mqttClient.UseConnectedHandler(async e =>
        {
            _logger.LogInformation("Connected to MQTT broker.");
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("test").Build());
        });

        _mqttClient.UseDisconnectedHandler(async e =>
        {
            _logger.LogInformation("Disconnected from MQTT broker.");
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            await _mqttClient.ConnectAsync(new MqttClientOptionsBuilder().WithTcpServer("localhost").Build(), cancellationToken);
        });

        _mqttClient.UseApplicationMessageReceivedHandler(e =>
        {
            _logger.LogInformation("Received message from MQTT broker.");
            _logger.LogInformation("Topic = {0}", e.ApplicationMessage.Topic);
            _logger.LogInformation("Payload = {0}", Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
        });

        return _mqttClient.ConnectAsync(new MqttClientOptionsBuilder().WithTcpServer("localhost").Build(), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("MqttBrokerService is stopping.");

        return _mqttClient.DisconnectAsync();
    }

}