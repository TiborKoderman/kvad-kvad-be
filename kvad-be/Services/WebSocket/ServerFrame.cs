public sealed class ServerFrame
{
  public required string Command { get; set; } //MESSAGE, ERROR
  public required string Topic { get; set; }
  public required object? Payload { get; set; }

}

