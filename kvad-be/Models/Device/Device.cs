using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;


public class Device
{
    public required Guid Id { get; set; }
    public PhysicalAddress? Mac { get; set; } = null!;
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public bool Virtual { get; set; } = false;
    public string Location { get; set; } = "";
    public string Type { get; set; } = "";
    public List<Group> Groups { get; set; } = [];
    public User? Owner { get; set; } // Make Owner nullable
    public required Guid OwnerId { get; set; }
    public DeviceState? State { get; set; } // Make nullable for seeding
    public DeviceInfo? Info { get; set; } // Make nullable for seeding
    public DeviceHeartbeatSettings? HeartbeatSettings { get; set; } // Make nullable for seeding
    public DeviceLifecycle Lifecycle { get; set; } = DeviceLifecycle.Unknown;
    public List<Tag> Tags { get; set; } = [];
}
[Owned]
public class DeviceHeartbeatSettings
{
    public int ExpectedInterval { get; set; } = 30;
    public int HbIntervalThreshold { get; set; } = 30 * 3;
    public int HbMissedThreshold { get; set; } = 3;
}


[JsonConverter(typeof(JsonStringEnumConverter))]
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