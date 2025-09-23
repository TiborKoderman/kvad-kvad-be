using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;

public class TopicHub
{
  private readonly ConcurrentDictionary<string, Topic> _topics = new(StringComparer.Ordinal);
  private readonly ConcurrentDictionary<Guid, WsClient> _clients = new();
  private readonly ILogger<TopicHub> _logger;
  private readonly AuthService _authService;

  public TopicHub(ILogger<TopicHub> logger, AuthService authService)
  {
    _logger = logger;
    _authService = authService;
  }

  public async Task ConnectClientAsync(HttpContext context)
  {
    string? token = context.Request.Query["token"];
    var user = await _authService.ValidateToken(token);
    if (user == null)
    {
      context.Response.StatusCode = 401; // Unauthorized
      return;
    }

    if (context.WebSockets.IsWebSocketRequest)
    {
      using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
      var client = new WsClient(webSocket, user);
      _clients[client.Id] = client;
    }
    else
    {
      context.Response.StatusCode = 400; // Bad Request
    }


  }
  public async Task ProcessFrame(WsClient client, Frame frame)
  {
    switch (frame.Command.ToUpper())
    {
      case "SUBSCRIBE":
      case "UNSUBSCRIBE":
      case "PUBLISH":
      case "PING":
        await HandlePing(client);
        break;
      case "PONG":
      case "DISCONNECT":
      case "ACK":
      case "NACK":
      case "BEGIN":
      case "COMMIT":
      case "ABORT":
      case "SEND":
      case "MESSAGE":
      case "RECEIPT":
      case "CREATE TOPIC":
      case "DELETE TOPIC":
        // Handle other commands as needed
        break;
      default:
        _logger.LogWarning("Unknown command: {Command}", frame.Command);
        break;
    }
  }

  private async Task SendError(WsClient client, string errorCode, string message)
  {
    var errorFrame = new Frame($"ERROR {errorCode}", "Message", new Dictionary<string, string>
    {
      { "message", message }
    });
    await SendFrame(client, errorFrame);
  }

  private async Task SendFrame(WsClient client, Frame frame)
  {
    if (client.Socket.State != WebSocketState.Open) return;

    var data = frame.ToArraySegment();
    await client.Socket.SendAsync(data, WebSocketMessageType.Text, true, client.Cancellation);
  }

  private async Task HandleSubscribe(WsClient client, Frame frame)
  {
    if (!frame.Headers.TryGetValue("topic", out var topic))
    {
      await SendError(client, "400", "Missing 'topic' header in SUBSCRIBE frame.");
      return;
    }

    client.Subscriptions.Add(topic);


    _topics.AddOrUpdate(topic, t => new Topic(t), (t, topicObj) =>
    {
      topicObj.Add(client);
      return topicObj;
    });

    var response = new Frame
    {
      Command = "SUBSCRIBED",
      Headers = { ["topic"] = topic }
    };
    await SendFrame(client, response);
    _logger.LogInformation("Client {ClientId} subscribed to {Topic}", client.Id, topic);
  }

  private async Task HandlePing(WsClient client)
  {
    var pong = new Frame
    {
      Command = "PONG",
      Headers = { ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
    };

    await SendFrame(client, pong);
  }
}