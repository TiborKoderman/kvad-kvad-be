using MQTTnet.Server;
using MQTTnet.Diagnostics.Logger;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MQTTnet.Protocol;
using System.Buffers;

public class MqttServerService(
    IConfiguration configuration,
    IServiceProvider serviceProvider,
    ILogger<MqttServerService> logger,
    JsonSerializerOptions jsonOptions) : BackgroundService
{
    private MqttServer? _mqttServer;
    private readonly string _certPath = configuration["MqttServer:CertPath"] ?? "server-cert.pfx";
    private readonly string _certPassword = configuration["MqttServer:CertPassword"] ?? "your_password";
    private readonly int _mqttPort = int.Parse(configuration["MqttServer:MqttPort"] ?? "1883");
    private readonly int _encryptedMqttPort = int.Parse(configuration["MqttServer:EncryptedMqttPort"] ?? "8883");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // EnsureCertificateExists();
        _mqttServer = await StartMqttServer();

        // Wait for the service to stop
        stoppingToken.Register(async () => await StopMqttServer());
    }

    private X509Certificate2 LoadOrGenerateCertificate()
    {
        if (File.Exists(_certPath))
        {
            Console.WriteLine("Loading existing certificate...");
            return X509CertificateLoader.LoadPkcs12(File.ReadAllBytes(_certPath), _certPassword);
        }

        Console.WriteLine("Certificate not found. Generating a new self-signed certificate...");
        using var rsa = RSA.Create(2048);
        var certificateRequest = new CertificateRequest(
            "CN=MQTT Server",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        certificateRequest.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, false));
        certificateRequest.CertificateExtensions.Add(
            new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
        certificateRequest.CertificateExtensions.Add(
            new X509SubjectKeyIdentifierExtension(certificateRequest.PublicKey, false));

        // Replace obsolete CreateSelfSigned with the active equivalent
        var certificate = certificateRequest.CreateSelfSigned(
            DateTimeOffset.Now.AddDays(-1),
            DateTimeOffset.Now.AddYears(5));

        var exportData = certificate.Export(X509ContentType.Pfx, _certPassword);
        File.WriteAllBytes(_certPath, exportData);

        Console.WriteLine("Self-signed certificate generated and saved.");
        return X509CertificateLoader.LoadPkcs12(exportData, _certPassword);
    }

    private async Task<MqttServer> StartMqttServer()
    {
        var mqttServerFactory = new MqttServerFactory();
        var certificate = LoadOrGenerateCertificate();

        var mqttServerOptions =
            mqttServerFactory.CreateServerOptionsBuilder()
                .WithEncryptedEndpoint()
                .WithEncryptedEndpointPort(_encryptedMqttPort)
                .WithEncryptionCertificate(certificate)
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(_mqttPort)
                .Build();

        var server = mqttServerFactory.CreateMqttServer(mqttServerOptions);

        // Subscribe to message events for heartbeat handling
        server.InterceptingPublishAsync += OnMessageReceived;

        await server.StartAsync();
        Console.WriteLine("MQTT server started with certificate.");
        return server;
    }

    private async Task OnMessageReceived(InterceptingPublishEventArgs eventArgs)
    {
        try
        {
            var topic = eventArgs.ApplicationMessage.Topic;
            var payload = string.Empty;

            if (!eventArgs.ApplicationMessage.Payload.IsEmpty)
            {
                var payloadBytes = eventArgs.ApplicationMessage.Payload.ToArray();
                payload = Encoding.UTF8.GetString(payloadBytes);
            }

            // Check if this is a heartbeat message
            if (IsHeartbeatTopic(topic))
            {
                await HandleHeartbeatMessage(topic, payload);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing MQTT message on topic: {Topic}", eventArgs.ApplicationMessage.Topic);
        }
    }

    private bool IsHeartbeatTopic(string topic)
    {
        // Match pattern: device/<deviceId>/hb
        return topic.StartsWith("device/") && topic.EndsWith("/hb");
    }

    private async Task HandleHeartbeatMessage(string topic, string payload)
    {
        try
        {
            // Extract device ID from topic: device/<deviceId>/hb
            var topicParts = topic.Split('/');
            if (topicParts.Length != 3 || !Guid.TryParse(topicParts[1], out var deviceId))
            {
                logger.LogWarning("Invalid heartbeat topic format: {Topic}", topic);
                return;
            }

            // Parse heartbeat payload
            var heartbeat = JsonSerializer.Deserialize<HeartbeatDTO>(payload, jsonOptions);
            if (heartbeat == null)
            {
                logger.LogWarning("Failed to deserialize heartbeat payload from device {DeviceId}", deviceId);
                return;
            }

            // Use scoped service to handle the heartbeat
            using var scope = serviceProvider.CreateScope();
            var heartbeatHandler = scope.ServiceProvider.GetRequiredService<DeviceHeartbeatHandlerService>();

            await heartbeatHandler.HandleHeartbeatAsync(deviceId, heartbeat);

            logger.LogDebug("Processed heartbeat for device {DeviceId} on topic {Topic}", deviceId, topic);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling heartbeat message from topic: {Topic}", topic);
        }
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
            return [];
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
            return [];
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