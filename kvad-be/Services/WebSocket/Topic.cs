using System.Collections.Concurrent;

sealed class Topic
{
  private readonly ConcurrentDictionary<Guid, WsClient> _subscribers = new();
  public string Key { get; }
  public Topic(string key) => Key = key;

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
            => _subscribers.Values.ToArray();
}