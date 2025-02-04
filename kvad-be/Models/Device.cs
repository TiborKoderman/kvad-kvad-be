using System.Net.NetworkInformation;
public class Device {
    public required PhysicalAddress Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    public required string Location { get; set; }

    public required string Type { get; set; }
}