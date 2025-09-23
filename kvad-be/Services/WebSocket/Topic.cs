using System.Collections.Concurrent;

sealed class Topic
{
  private readonly ConcurrentDictionary<Guid, WsConnection> _subscribers = new();
  public string Key { get; }
  public Topic(string key) => Key = key;

public void Add(WsConnection connection)
  {
    _subscribers[connection.Id] = connection;
    connection.CurrentTopics.Add(Key);
  }

  public void Remove(WsConnection connection)
  {
    _subscribers.TryRemove(connection.Id, out _);
    connection.CurrentTopics.Remove(Key);
  }

  public IReadOnlyCollection<WsConnection> GetSubscribers()
            => _subscribers.Values.ToArray();
}