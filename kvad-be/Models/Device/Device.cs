using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
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
    public List<Tag> Tags { get; set; } = [];
}
