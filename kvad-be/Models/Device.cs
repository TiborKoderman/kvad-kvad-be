using System.Net.NetworkInformation;
public class Device {
    public required Guid Id { get; set; }
    public required PhysicalAddress Mac { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    public required string Location { get; set; }

    public required string Type { get; set; }
}