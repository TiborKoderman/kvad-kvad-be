using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(DeviceId), nameof(Id))]
public class Tag
{
    public required Guid DeviceId { get; set; }
    public required Device Device { get; set; }
    public required string Id { get; set; }
    public required int SeriesId { get; set; } // unique global identifier
    public Unit? Unit { get; set; }
    public int? UnitId { get; set; }
    public required TagSource Source { get; set; }
    public required ValueType ValueType { get; set; }
    public required bool Enabled { get; set; } = true;
    public required bool Historize { get; set; } = false;
    public required TagCurr Curr { get; set; } = null!;
}

public enum ValueType
{
    Float,
    Double,
    Int,
    Bool,
    Enum,
    String,
    Json
}