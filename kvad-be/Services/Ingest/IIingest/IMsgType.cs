public interface IMsgType
{
  public string Name { get; }
  public string Slug { get; }
}

enum MsgTypeEnum
{
  Heartbeat,
  Telemetry,
  Event,
  Alert
}