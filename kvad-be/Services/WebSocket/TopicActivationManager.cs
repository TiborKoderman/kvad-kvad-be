using System.Collections.Concurrent;
namespace kvad_be.Services.WebSocket;

internal sealed class TopicActivationManager
{
  private sealed class Activation
  {
    public int RefCount;
    public IDisposable? Handle;
  }

  private readonly ConcurrentDictionary<string, Activation> _map = new(StringComparer.Ordinal);

  /// <summary>
  /// Increment refcount; if this is the first, create and store the publisher handle.
  /// The factory must return an IDisposable that stops the publisher when disposed.
  /// </summary>
  public void AddRef(string topic, Func<string, IDisposable> startFactory)
  {
    var act = _map.GetOrAdd(topic, _ => new Activation());
    lock (act)
    {
      if (act.RefCount == 0)
      {
        act.Handle = startFactory(topic);
      }
      act.RefCount++;
    }
  }

  /// <summary>
  /// Decrement refcount; if it hits zero, dispose the publisher handle and remove entry.
  /// </summary>
  public void Release(string topic)
  {
    if (!_map.TryGetValue(topic, out var act)) return;
    bool drop = false;
    IDisposable? toDispose = null;

    lock (act)
    {
      if (act.RefCount > 0) act.RefCount--;
      if (act.RefCount == 0)
      {
        toDispose = act.Handle;
        act.Handle = null;
        drop = true;
      }
    }

    toDispose?.Dispose();
    if (drop) _map.TryRemove(topic, out _);
  }

  public int GetRefCount(string topic)
    => _map.TryGetValue(topic, out var act) ? act.RefCount : 0;
}
