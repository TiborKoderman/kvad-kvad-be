public sealed class ClientFrame
{
  public required string Command { get; set; } //SEND, SUB, UNSUB, DISCONNECT
  public required string Topic { get; set; }
  public required object? Payload { get; set; }
}