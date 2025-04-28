using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
public class Device {
    public required Guid Id { get; set; }
    public PhysicalAddress? Mac { get; set; } = null!;
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public bool Virtual { get; set; } = false;
    public string Location { get; set; } = "";
    public string Type { get; set; } = "";
    public List<Group> Groups { get; set; } = [];

    public required User Owner { get; set; }
    public required DeviceState State { get; set; }
    public List<Tag> Tags { get; set; } = [];

}