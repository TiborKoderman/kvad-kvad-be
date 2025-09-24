using System.Net.WebSockets;

public class WsClient(WebSocket Socket, User? User = null)
{
  public Guid Id { get; } = Guid.NewGuid();
  public WebSocket Socket { get; set; } = Socket;
  public User? User { get; set; } = User;
  public HashSet<string> Subscriptions { get; } = new(StringComparer.Ordinal);


  private readonly CancellationTokenSource _cts = new();
  public CancellationTokenSource CancellationSource => _cts;
  public CancellationToken Cancellation => _cts.Token;

  public void MarkClosed() => _cts.Cancel();

}