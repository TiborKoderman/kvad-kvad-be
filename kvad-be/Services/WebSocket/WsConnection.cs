using System.Net.WebSockets;

public sealed class WsConnection
{
  public Guid Id { get; } = Guid.NewGuid();
  public required WebSocket Socket { get; set; }
  public Guid? UserId { get; set; }
  public HashSet<string> CurrentTopics { get; } = new(StringComparer.Ordinal);


  private readonly CancellationTokenSource _cts = new();
  public CancellationTokenSource CancellationSource => _cts;
  public CancellationToken Cancellation => _cts.Token;

  public WsConnection(WebSocket socket, Guid? userId)
  {
    Socket = socket;
    UserId = userId;
  }

  public void MarkClosed() => _cts.Cancel();

}