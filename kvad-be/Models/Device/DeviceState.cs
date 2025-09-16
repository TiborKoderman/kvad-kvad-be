using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using NodaTime;

public class DeviceState
{
    [Key, ForeignKey(nameof(Device))] public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    public Instant? LastHeartbeat { get; set; } = null;
    public string? BootId { get; set; } = null;
    public long? Seq { get; set; } = 0;
    public int? UptimeSec { get; set; } = 0;

    public int? HbIntervalSec { get; set; } = null;
    public short? HbJitterPct { get; set; } = null;

    public string? ConfigHash { get; set; } = null;

    // Optional health (cheap metrics only)
    public string? LastIp { get; set; }
    public int? Rssi { get; set; }
    public short? BatteryPct { get; set; }
    public short? LoadPct { get; set; }
    public float? TempC { get; set; }

    public DeviceLifecycle Lifecycle { get; set; } = DeviceLifecycle.Unknown;
    public DeviceMode Mode { get; set; } = DeviceMode.Unknown;
    public DeviceConnectivity Connectivity { get; set; } = DeviceConnectivity.Unknown;
    public DeviceHealth Health { get; set; } = DeviceHealth.Unknown;

    [Column(TypeName = "jsonb")] public JsonDocument? Flags { get; set; }   // e.g., ["ok","throttled"]
    [Column(TypeName = "jsonb")] public JsonDocument? Extra { get; set; } 

    public Instant UpdatedAt { get; set; } = SystemClock.Instance.GetCurrentInstant();
}

public enum DeviceLifecycle
{
    Unknown,
    Discovered,
    Commissioning,
    InService,
    OutOfService,
    Decommissioned,
    Retired,
}
public enum DeviceConnectivity
{
    Online,
    Offline,
    Intermittent,
    Unreachable,
    Unknown
}

public enum DeviceHealth
{
    Healthy,
    Warning,
    Critical,
    Unknown
}

public enum DeviceMode
{
    Normal,
    Maintenance,
    Emergency,
    Test,
    Unknown
}
