using System.Net.NetworkInformation;
public class Device {
    public required Guid Id { get; set; }
    public PhysicalAddress Mac { get; set; } = null!;
    public required string Name { get; set; }
    public string Description { get; set; } = null!;
    public string Location { get; set; } = null!;
    public string Type { get; set; } = null!;
}