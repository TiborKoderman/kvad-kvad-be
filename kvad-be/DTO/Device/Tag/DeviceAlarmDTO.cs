public sealed record DeviceAlarmDTO(
    int? TagId,
    string Message,
    DateTime Timestamp,
    TagQuality Quality
);