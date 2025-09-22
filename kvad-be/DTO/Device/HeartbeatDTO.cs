public record HeartbeatDTO(
    long Ts,
    string BootId,
    long Seq,
    long UptimeS,
    string? CfgHash,
    string?[] Flags,
    string?[] Extra
);
