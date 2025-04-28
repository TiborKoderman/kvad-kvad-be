using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(DeviceId), nameof(Id))]
public class Tag
{
    public required Guid DeviceId { get; set; }
    public required Device Device { get; set; }
    public required string Id { get; set; }
    public required Unit Unit { get; set; }
    public required string Source { get; set; } = ""; //Virtual, mqtt, computed, modbus
    public required bool Virtual { get; set; } = false;
    public required bool Enabled { get; set; } = true;
    public required bool Historicize { get; set; } = false;
}