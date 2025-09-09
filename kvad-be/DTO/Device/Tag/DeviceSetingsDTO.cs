public sealed record DeviceSettingsDTO(
    DeviceSettingsHbDTO Heartbeat
);

public sealed record DeviceSettingsHbDTO
{
    public required int IntervalSeconds { get; set; }
    public required int JitterPct { get; set; }
}

public sealed record DeviceSettingsAlarmDTO
(
  string? TagPath,
  string? Message,
  TagQuality Quality
);