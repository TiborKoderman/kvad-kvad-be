using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TopicHub
{
  private readonly ConcurrentDictionary<string, Topic> _topics = new(StringComparer.Ordinal);
  private readonly ConcurrentDictionary<Guid, WsClient> _clients = new();
  private readonly ILogger<TopicHub> _logger;
  private readonly AuthService _authService;

  private static readonly TimeSpan ReceiveLoopPingInterval = TimeSpan.FromSeconds(30);

  public TopicHub(ILogger<TopicHub> logger, AuthService authService)
  {
    _logger = logger;
    _authService = authService;
  }

  public async Task ConnectClientAsync(HttpContext context)
  {
    var user = await _authService.GetUser(context.User);

    if (user == null)
    {
      context.Response.StatusCode = 401; // Unauthorized
      return;
    }

    if (!context.WebSockets.IsWebSocketRequest)
    {
      context.Response.StatusCode = 400;
      return;
    }

    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    var client = new WsClient(webSocket, user);
    _clients[client.Id] = client;
    _logger.LogInformation("Client connected: {ClientId}, User: {UserId}", client.Id, user.Id);

    try
    {
      await SendGreeting(client);
      await ReceiveLoop(client);
    }
    catch (Exception ex)
    {
      _logger.LogWarning(ex, "Receive loop error for client {ClientId}", client.Id);
    }
    finally
    {
      //await DisconnectClient(client);
    }
  }

  private async Task SendGreeting(WsClient client)
  {
    var greeting = new Frame(Command.CONNECTED, new Dictionary<string, string>
    {
      { "Timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
      { "Server", "Kvad WebSocket Server v1.0" }
    });
    await SendFrame(client, greeting);
  }

  private async Task ReceiveLoop(WsClient client)
  {
    var socket = client.Socket;
    var buffer = new byte[4096];

    // Optional: background ping to keep idle connections alive
    using var pingCts = CancellationTokenSource.CreateLinkedTokenSource(client.Cancellation);
    _ = Task.Run(async () =>
    {
      try
      {
        while (!pingCts.IsCancellationRequested && socket.State == WebSocketState.Open)
        {
          await Task.Delay(ReceiveLoopPingInterval, pingCts.Token);
          if (socket.State != WebSocketState.Open) break;
          var ping = new Frame(Command.PING);
          await SendFrame(client, ping);
        }
      }
      catch { /* ignore */ }
    }, pingCts.Token);


    try
    {
      while (socket.State == WebSocketState.Open && !client.Cancellation.IsCancellationRequested)
      {
        using var ms = new MemoryStream();
        WebSocketReceiveResult? result;
        do
        {
          result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), client.Cancellation);
          if (result.MessageType == WebSocketMessageType.Close)
          {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", CancellationToken.None);
            return;
          }
          if (result.Count > 0)
            ms.Write(buffer, 0, result.Count);
        }
        while (!result.EndOfMessage);

        var raw = ms.ToArray();

        // Our codec expects COMMAND/headers text then optional payload.
        // We parse regardless of incoming message type.
        var frame = Frame.Parse(raw);
        await ProcessFrame(client, frame);
      }
    }
    finally
    {
      pingCts.Cancel();
    }
  }

  private async Task DisconnectClient(WsClient client)
  {
    foreach (var topicKey in client.Subscriptions)
    {
      if (_topics.TryGetValue(topicKey, out var topic))
      {
        topic.Remove(client);
        if (topic.IsEmpty)
        {
          _topics.TryRemove(topicKey, out _);
        }
      }
    }
    _clients.TryRemove(client.Id, out _);
    try { client.MarkClosed(); } catch { /* ignore */ }

    if (client.Socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
    {
      try { await client.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "disconnect", CancellationToken.None); }
      catch { /* ignore */ }
    }

    _logger.LogInformation("Client {ClientId} disconnected", client.Id);
  }


  public async Task ProcessFrame(WsClient client, Frame frame)
  {
    switch (frame.Command)
    {
      case Command.SUBSCRIBE:
        await HandleSubscribe(client, frame);
        break;
      case Command.UNSUBSCRIBE:
        await HandleUnsubscribe(client, frame);
        break;
      case Command.PUBLISH:
        await HandlePublish(client, frame);
        break;
      case Command.PING:
        await HandlePing(client);
        break;
      case Command.DISCONNECT:
        await DisconnectClient(client);
        break;
      case Command.NACK:
      case Command.BEGIN:
      case Command.COMMIT:
      case Command.ABORT:
      case Command.SEND:
      case Command.MESSAGE:
      case Command.RECEIPT:
        // Handle other commands as needed
        break;
      default:
        _logger.LogWarning("Unknown command: {Command}", frame.Command);
        break;
    }
  }

  private async Task SendError(WsClient client, string errorCode, string message)
  {
    var errorFrame = new Frame(Command.ERROR, message, new Dictionary<string, string>
    {
      { "code", errorCode }
    });
    await SendFrame(client, errorFrame);
  }

  private async Task SendOk(WsClient client, Command command, string? message)
  {
    var okFrame = new Frame(command, message ?? string.Empty, new Dictionary<string, string>
    {
      ["Status"] = StatusCode.OK
    });
    await SendFrame(client, okFrame);
  }


  private async Task SendFrame(WsClient client, Frame frame)
  {
    if (client.Socket.State != WebSocketState.Open) return;

    var data = frame.ToArraySegment();
    var msgType = frame.GetSuggestedMessageType();
    await client.Socket.SendAsync(data, WebSocketMessageType.Text, true, client.Cancellation);
  }

  private async Task HandleSubscribe(WsClient client, Frame frame)
  {
    if (!frame.Headers.TryGetValue("Topic", out var topic))
    {
      await SendError(client, StatusCode.BadRequest, "Missing 'topic' header in SUBSCRIBE frame.");
      return;
    }

    // Optional scoping: if client wants per-user isolation, allow `Scope=user`
    if (frame.Headers.TryGetValue("Scope", out var scope))
    {
      switch (scope.ToLowerInvariant())
      {
        case "user":
          if (client.User == null)
          {
            await SendError(client, StatusCode.Unauthorized, "Authentication required for user-scoped topics.");
            return;
          }
          topic = $"/u/{client.User.Id}/{topic}";
          break;
        default:
          await SendError(client, StatusCode.BadRequest, $"Unknown scope '{scope}' in SUBSCRIBE frame.");
          return;
      }
    }

    var t = _topics.GetOrAdd(topic, static k => new Topic(k));
    t.Add(client);

    var response = new Frame
    {
      Command = Command.SUBSCRIBED,
      Headers = { ["Topic"] = topic, ["Status"] = StatusCode.OK }
    };
    await SendFrame(client, response);
    _logger.LogInformation("Client {ClientId} subscribed to {Topic}", client.Id, topic);
  }


  private async Task HandleUnsubscribe(WsClient client, Frame frame)
  {
    if (!frame.Headers.TryGetValue("Topic", out var topic))
    {
      await SendError(client, StatusCode.BadRequest, "Missing 'topic' header in UNSUBSCRIBE frame.");
      return;
    }

    // Mirror same scoping rule
    if (frame.Headers.TryGetValue("Scope", out var scope) &&
        scope.Equals("user", StringComparison.OrdinalIgnoreCase) &&
        client.User is not null)
    {
      topic = $"u/{client.User.Id}/{topic}";
    }

    if (_topics.TryGetValue(topic, out var t))
    {
      t.Remove(client);
      if (t.IsEmpty) _topics.TryRemove(topic, out _);
    }

    await SendFrame(client, new Frame
    {
      Command = Command.UNSUBSCRIBED,
      Headers = { ["Topic"] = topic, ["Status"] = StatusCode.OK }
    });

    _logger.LogInformation("Client {ClientId} unsubscribed from {Topic}", client.Id, topic);
  }

  private async Task HandlePublish(WsClient client, Frame frame)
  {
    if (!frame.Headers.TryGetValue("Topic", out var topic) || string.IsNullOrWhiteSpace(topic))
    {
      await SendError(client, StatusCode.BadRequest, "Missing 'topic' header in PUBLISH.");
      return;
    }

    // Optional per-user scoping for publish as well
    if (frame.Headers.TryGetValue("Scope", out var scope) &&
        scope.Equals("user", StringComparison.OrdinalIgnoreCase) &&
        client.User is not null)
    {
      topic = $"u/{client.User.Id}/{topic}";
    }

    if (!_topics.TryGetValue(topic, out var t) || t.IsEmpty)
    {
      await SendFrame(client, new Frame(Command.PUBLISHED, "No subscribers for topic.", new Dictionary<string, string>
      {
        ["Topic"] = topic,
        ["code"] = StatusCode.OK,
        ["delivered"] = "0"
      }));
      _logger.LogInformation("Publish to {Topic} had no subscribers", topic);
      return;
    }

    // Build delivery frame (MESSAGE) preserving payload and key headers
    var deliveryHeaders = new Dictionary<string, string>(StringComparer.Ordinal)
    {
      ["Topic"] = topic,
      ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
    };

    foreach (var k in new[] { "DataType", "ContentType", "ContentLength", "Type" })
      if (frame.Headers.TryGetValue(k, out var v)) deliveryHeaders[k] = v;

    if (client.User != null)
      deliveryHeaders["From"] = client.User.Id.ToString();

    var message = new Frame(Command.MESSAGE, frame.Payload, deliveryHeaders);

    int delivered = 0;
    foreach (var subscriber in t.GetSubscribers())
    {
      try
      {
        await SendFrame(subscriber, message);
        delivered++;
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Failed to deliver message to client {ClientId}", subscriber.Id);
      }
    }

    await SendFrame(client, new Frame(Command.PUBLISHED, "Message published.", new Dictionary<string, string>
    {
      ["Topic"] = topic,
      ["code"] = StatusCode.OK,
      ["delivered"] = delivered.ToString()
    }));

    _logger.LogInformation("Published message to {Topic}, delivered to {Count} subscribers", topic, delivered);

  }

  private async Task HandlePing(WsClient client)
  {
    var pong = new Frame
    {
      Command = Command.PONG,
      Headers = { ["Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
    };

    await SendFrame(client, pong);
  }



  public async Task<int> PublishAsync(string topic, ReadOnlyMemory<byte> payload, Dictionary<string, string>? headers = null, bool scopePerUser = false, User? fromUser = null)
  {
    var headerBag = new Dictionary<string, string>(StringComparer.Ordinal)
    {
      ["topic"] = topic
    };
    if (headers != null)
      foreach (var kv in headers) headerBag[kv.Key] = kv.Value;

    if (headerBag.TryGetValue("DataType", out var dt) == false)
      headerBag["DataType"] = "binary"; // default for raw payload
    if (string.Equals(headerBag["DataType"], "binary", StringComparison.OrdinalIgnoreCase))
      headerBag["ContentLength"] = payload.Length.ToString();
    if (fromUser != null)
      headerBag["From"] = fromUser.Id.ToString();

    var topicKey = scopePerUser && fromUser != null
  ? $"{fromUser.Id}:{topic}"
  : topic;

    if (!_topics.TryGetValue(topicKey, out var t) || t.IsEmpty)
      return 0;
    var msg = new Frame
    {
      Command = Command.MESSAGE,
      Headers = headerBag,
      Payload = payload
    };

    int delivered = 0;
    foreach (var sub in t.GetSubscribers())
    {
      if (sub.Socket.State != WebSocketState.Open) continue;
      try { await SendFrame(sub, msg); delivered++; }
      catch { /* ignore a single failure */ }
    }

    return delivered;
  }
  // Convenience helpers
  public Task<int> PublishTextAsync(string topic, string text, Dictionary<string, string>? headers = null)
    => PublishAsync(topic, Encoding.UTF8.GetBytes(text),
         headers ?? new(StringComparer.Ordinal) { ["DataType"] = "text" });

  public Task<int> PublishJsonAsync<T>(string topic, T payload, Dictionary<string, string>? headers = null) where T : notnull
  {
    var h = headers ?? new(StringComparer.Ordinal);
    h["DataType"] = "json";
    h["ContentType"] = "application/json";
    h["Type"] = typeof(T).Name;
    return PublishAsync(topic, JsonSerializer.SerializeToUtf8Bytes(payload), h);
  }

}
