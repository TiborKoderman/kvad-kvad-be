using NpgsqlTypes;

public enum CaptureMode
{
    OnChange,
    Periodic,
}

public class TagHistPolicy
{
  public required int Id { get; set; }
  public required int TagId { get; set; }
  public required bool Enabled { get; set; } = false;

  // [from, to) — no overlaps per TagId
  public required NpgsqlRange<DateTimeOffset> ValidRange { get; set; } //Check for no overlap
  public required CaptureMode CaptureMode { get; set; }

  // For CaptureMode.OnChange and CaptureMode.Both
  public double? AbsDeadband { get; set; } = null;
  public double? PercentDeadband { get; set; } = null;
  public short? RoundDigits { get; set; } = null;

  // For CaptureMode.OnInterval
  public TimeSpan? SamplingInterval { get; set; } = null;
  public bool Aggregate { get; set; } = false;
}