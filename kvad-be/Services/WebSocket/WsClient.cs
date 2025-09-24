namespace kvad_be.Services.WebSocket;

public class WsClient(System.Net.WebSockets.WebSocket Socket, User? User = null) : IDisposable
{
  public Guid Id { get; } = Guid.NewGuid();
  public System.Net.WebSockets.WebSocket Socket { get; set; } = Socket;
  public User? User { get; set; } = User;
  public HashSet<string> Subscriptions { get; } = new(StringComparer.Ordinal);


  private readonly CancellationTokenSource _cts = new();
  public CancellationTokenSource CancellationSource => _cts;
  public CancellationToken Cancellation => _cts.Token;

  public void MarkClosed() => _cts.Cancel();

  private bool _disposed;

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposed)
    {
      if (disposing)
      {
        _cts.Dispose();
        // Dispose managed resources here if needed
      }
      // Free unmanaged resources here if needed
      _disposed = true;
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}