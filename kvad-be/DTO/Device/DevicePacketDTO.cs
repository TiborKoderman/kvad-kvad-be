using NodaTime;

public sealed record DevicePacketDTO(
    Instant Timestamp,
    IReadOnlyList<TagIDTO> Tags
);