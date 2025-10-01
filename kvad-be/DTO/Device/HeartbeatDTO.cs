
public record HeartbeatDTO(
    long Ts,
    string BootId,
    long Seq,
    long UptimeS,
    long? Rssi,
    string? Ip,
    string? CfgHash,
    string?[] Flags,
    string?[] Extra
);
