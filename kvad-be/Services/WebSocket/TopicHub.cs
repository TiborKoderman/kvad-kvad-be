using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;


public sealed class TopicHub
{
  private readonly ConcurrentDictionary<string, Topic> _topics = new(StringComparer.Ordinal);

  public void Subscribe(string topicKey, WsConnection connection)
  {
    var topic = _topics.GetOrAdd(topicKey, key => new Topic(key));
    topic.Add(connection);
  }

  public void Unsubscribe(string topicKey, WsConnection connection)
  {
    if (_topics.TryGetValue(topicKey, out var topic))
    {
      topic.Remove(connection);
      if (topic.GetSubscribers().Count == 0)
      {
        _topics.TryRemove(topicKey, out _);
      }
    }
  }

  public IReadOnlyCollection<WsConnection> GetSubscribers(string topicKey)
  {
    return _topics.TryGetValue(topicKey, out var topic) ? topic.GetSubscribers() : Array.Empty<WsConnection>();
  }


  private async Task HandleFrame(WebSocket socket, ClientFrame frame)
  {
    switch (frame.Command.ToUpperInvariant())
    {
      case "SUB":
        Subscribe(frame.Topic, new WsConnection(socket, null));
        break;
      case "UNSUB":
        Unsubscribe(frame.Topic, new WsConnection(socket, null));
        break;
      case "SEND":
        var subscribers = GetSubscribers(frame.Topic);
        var message = JsonSerializer.SerializeToUtf8Bytes(frame.Payload);
        foreach (var subscriber in subscribers)
        {
          if (subscriber.Socket.State == WebSocketState.Open)
          {
            await subscriber.Socket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
          }
        }
        break;
      case "DISCONNECT":
        // Handle disconnection logic if needed
        break;
      default:
        throw new InvalidOperationException($"Unknown command: {frame.Command}");
    }
  }
  private static async Task SendAsync(WebSocket socket, Frame frame)
  {
    if (socket.State != WebSocketState.Open) return;
    var bytes = FrameCodec.Serialize(frame);
    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
  }

}