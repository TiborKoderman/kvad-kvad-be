using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DeviceState
{
    [Key, ForeignKey(nameof(Device))] public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    public DateTime? LastSeen { get; set; } = null;
}

enum DeviceLifecycle
{
    Unknown,
    Discovered,
    Commissioning,
    InService,
    OutOfService,
    Decommissioned,
    Retired,
}
enum DeviceConnectivity
{
    Online,
    Offline,
    Intermittent,
    Unreachable,
    Unknown
}

enum DeviceHealth
{
    Healthy,
    Warning,
    Critical,
    Unknown
}

enum DeviceMode
{
    Normal,
    Maintenance,
    Emergency,
    Test,
    Unknown
}
