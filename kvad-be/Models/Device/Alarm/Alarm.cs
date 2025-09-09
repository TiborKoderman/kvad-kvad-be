public class Alarm
{
  public required long Id { get; set; }
  public required Guid DeviceId { get; set; }

  public Tag[] Tags { get; set; } = [];
  public required string Name { get; set; }
  public string Description { get; set; } = "";
  public required bool Enabled { get; set; } = true;

}

public enum AlarmKind
{
  Device,
  System,
  Tag

}

public enum AlarmState
{
  Active,
  Acknowledged,
  Cleared,
  Shelved,
  Expired
}

public enum AlarmSeverity
{
  Info,
  Warning,
  Minor,
  Major,
  Critical
}