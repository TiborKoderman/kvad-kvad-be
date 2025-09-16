using NodaTime;

public sealed record DeviceStateDTO(
    Guid DeviceId,
    DeviceConnectivity Connectivity,
    DeviceHealth Health,
    DeviceMode Mode,
    Instant? LastHeartbeat
);