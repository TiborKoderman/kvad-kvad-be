using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;
using MQTTnet.Diagnostics.Logger;

namespace MQTTnetBackgroundService
{
    public class MqttBackgroundService : BackgroundService
    {
        private MqttServer? _mqttServer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _mqttServer = await StartMqttServer();

            // Wait for the service to stop
            stoppingToken.Register(async () => await StopMqttServer());
        }

        private async Task<MqttServer> StartMqttServer()
        {
            var mqttServerFactory = new MqttServerFactory();
            var mqttServerOptions = mqttServerFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();
            var server = mqttServerFactory.CreateMqttServer(mqttServerOptions);
            await server.StartAsync();
            Console.WriteLine("MQTT server started.");
            return server;
        }

        private async Task StopMqttServer()
        {
            if (_mqttServer != null)
            {
                await _mqttServer.StopAsync();
                Console.WriteLine("MQTT server stopped.");
            }
        }
    }

    public class ConsoleLogger : IMqttNetLogger
    {
        private readonly object _consoleSyncRoot = new();
        public bool IsEnabled => true;

        public void Publish(MqttNetLogLevel logLevel, string source, string message, object[]? parameters, Exception? exception)
        {
            var foregroundColor = logLevel switch
            {
                MqttNetLogLevel.Verbose => ConsoleColor.White,
                MqttNetLogLevel.Info => ConsoleColor.Green,
                MqttNetLogLevel.Warning => ConsoleColor.DarkYellow,
                MqttNetLogLevel.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            if (parameters?.Length > 0)
            {
                message = string.Format(message, parameters);
            }

            lock (_consoleSyncRoot)
            {
                Console.ForegroundColor = foregroundColor;
                Console.WriteLine(message);

                if (exception != null)
                {
                    Console.WriteLine(exception);
                }
            }
        }
    }
}
