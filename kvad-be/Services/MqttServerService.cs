using MQTTnet.Server;
using MQTTnet.Diagnostics.Logger;
using System.Security.Cryptography.X509Certificates;

public class MqttServerService : BackgroundService
{
    private MqttServer? _mqttServer;
    private readonly string _certPath;
    private readonly string _certPassword;
    private readonly int _mqttPort;
    private readonly int _encryptedMqttPort;

    public MqttServerService(IConfiguration configuration)
    {
        _certPath = configuration["MqttServer:CertPath"] ?? "server-cert.pfx";
        _certPassword = configuration["MqttServer:CertPassword"] ?? "your_password";
        _mqttPort = int.Parse(configuration["MqttServer:MqttPort"] ?? "8883");
        _encryptedMqttPort = int.Parse(configuration["MqttServer:EncryptedMqttPort"] ?? "8884");
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // EnsureCertificateExists();
        _mqttServer = await StartMqttServer();

        // Wait for the service to stop
        stoppingToken.Register(async () => await StopMqttServer());
    }

    private async Task<MqttServer> StartMqttServer()
    {
        var mqttServerFactory = new MqttServerFactory();
        var mqttServerOptions =
        mqttServerFactory.CreateServerOptionsBuilder().
        WithDefaultEndpoint().
        WithDefaultEndpointPort(_mqttPort).
        Build();
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


    public async Task<List<MqttClientStatus>> GetAllActiveClients()
    {
        if (_mqttServer == null)
        {
            return new List<MqttClientStatus>();
        }
        IList<MqttClientStatus> clients = await _mqttServer.GetClientsAsync();



        Console.WriteLine("Active clients:");

        foreach (var client in clients)
        {
            Console.WriteLine($"Client ID: {client.Id}");
        }

        return clients.ToList();
    }

    public async Task<List<MqttSessionStatus>> GetAllActiveSessions()
    {
        if (_mqttServer == null)
        {
            return new List<MqttSessionStatus>();
        }
        IList<MqttSessionStatus> sessions = await _mqttServer.GetSessionsAsync();

        foreach (var session in sessions)
        {
            Console.WriteLine($"Session ID: {session.Id}");
        }

        return sessions.ToList();
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