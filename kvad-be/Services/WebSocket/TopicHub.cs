using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Claims;

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
      case "PONG":
      case "DISCONNECT":
      case "ACK":
      case "NACK":
      case "BEGIN":
      case "COMMIT":
      case "ABORT":
      default:
        _logger.LogWarning("Unknown command: {Command}", frame.Command);
        break;
    }
  }

  private async Task SendError(WsClient client, string errorCode, string message)
  {
    var errorFrame = new Frame
    {
      Command = $"ERROR {errorCode}",
      Headers = new Dictionary<string, string>
      {
        { "message", message }
      },
      Body = message
    };
    await SendFrameAsync(client, errorFrame);
  }

  private async Task HandleSubscribe(WsClient client, Frame frame)
  {
    if (!frame.Headers.TryGetValue("topic", out var topic))
    {
      await SendErrorAsync(client, "Missing 'topic' header in SUBSCRIBE frame.");
      return;
    }

    var topicObj = _topics.GetOrAdd(topic, key => new Topic(key));
    topicObj.Add(client);
    await SendReceiptAsync(client, frame);
  }
}