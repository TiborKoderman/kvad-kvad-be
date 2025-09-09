using NodaTime;

public record HeartbeatDTO(
    Instant Ts,
    Guid BootId,
    long Seq,
    long UptimeS,
    string? CfgHash,
    string?[] Flags,
    string?[] Extra
);
