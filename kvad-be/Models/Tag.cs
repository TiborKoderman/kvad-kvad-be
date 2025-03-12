using System.ComponentModel.DataAnnotations;

[PrimaryKey("ObisId", AutoIncrement = false)]
public class Tag{
    public required string ObisId { get; set; }
    public required Obis Obis { get; set; }
    public required Device Device { get; set; }
    public required int DeviceId { get; set; }
    public required Unit Unit { get; set; }
}