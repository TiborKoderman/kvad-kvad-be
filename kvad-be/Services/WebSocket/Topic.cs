using System.Collections.Concurrent;
namespace kvad_be.Services.WebSocket;

sealed class Topic(string key)
{
  private readonly ConcurrentDictionary<Guid, WsClient> _subscribers = new();
  public string Key { get; } = key;

  public void Add(WsClient connection)
  {
    _subscribers[connection.Id] = connection;
    connection.Subscriptions.Add(Key);
  }

  public void Remove(WsClient connection)
  {
    _subscribers.TryRemove(connection.Id, out _);
    connection.Subscriptions.Remove(Key);
  }

  public IReadOnlyCollection<WsClient> GetSubscribers()
            => [.. _subscribers.Values];

  public bool IsEmpty => _subscribers.IsEmpty;
}