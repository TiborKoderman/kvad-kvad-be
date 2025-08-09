using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(DeviceId), nameof(Id))]
public class Tag
{
    public required Guid DeviceId { get; set; }
    public required Device Device { get; set; }
    public required string Id { get; set; }
    public Unit? Unit { get; set; }
    public int? UnitId { get; set; }
    public required TagSource Source { get; set; }
    public required string Expression { get; set; } = ""; //Expression to compute the value
    public required bool Enabled { get; set; } = true;
    public required bool Historize { get; set; } = false;
    public required TagCurr Curr { get; set; } = null!;
}