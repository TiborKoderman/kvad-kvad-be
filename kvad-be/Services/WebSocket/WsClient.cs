using System.Net.WebSockets;

public class WsClient
{
  public Guid Id { get; } = Guid.NewGuid();
  public WebSocket Socket { get; set; }
  public User? User { get; set; }
  public HashSet<string> Subscriptions { get; } = new(StringComparer.Ordinal);
  public bool IsAuthenticated { get; set; }


  private readonly CancellationTokenSource _cts = new();
  public CancellationTokenSource CancellationSource => _cts;
  public CancellationToken Cancellation => _cts.Token;

  public WsClient(WebSocket socket, User? user)
  {
    Socket = socket;
    User = user;
  }

  public void MarkClosed() => _cts.Cancel();

}