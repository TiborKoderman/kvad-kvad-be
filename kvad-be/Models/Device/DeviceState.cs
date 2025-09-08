using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DeviceState
{
    [Key, ForeignKey(nameof(Device))]
    public Guid DeviceId { get; set; }

    public bool Online { get; set; } = false;
    public bool Connected { get; set; } = false;
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

enum DeviceQuality
{
    Good,
    Uncertain,
    Bad,
    Unknown
}