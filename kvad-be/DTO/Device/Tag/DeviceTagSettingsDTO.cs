public sealed record DeviceTagSettingsDTO(
  string Path,
  string ValueKind,
  string? Unit,
  string IO = "Output",
  DeviceTagHistorySettingsDTO? History

);

public sealed record DeviceTagHistorySettingsDTO(
  string CaptureMode, // e.g., "OnChange", "Periodic"
  int? PeriodSeconds, // Applicable if CaptureMode is "Periodic"
  float? AbsDeadband, // Optional absolute deadband
  float? PercentDeadband, // Optional percentage deadband
  int? RoundDigits, // Optional rounding precision
  bool Aggregate // Whether to aggregate data points
);

public sealed record DeviceTagEngineeringLimitsDTO(
  double? Min,
  double? Max
);