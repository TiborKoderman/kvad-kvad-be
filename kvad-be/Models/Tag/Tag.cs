using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(DeviceId), nameof(Id))]
public class Tag{
    public required Guid DeviceId { get; set; }
    public required Device Device { get; set; }
    public required string Id { get; set; }
    public required Unit Unit { get; set; }
}