using System.ComponentModel.DataAnnotations.Schema;

public class TagSource
{
    public required int TagId { get; set; } //one to one with Tag
    [ForeignKey(nameof(TagId))]
    public required Tag Tag { get; set; }
    public required TagSourceKind Kind { get; set; }
    public required string Name { get; set; } = "";
}


public enum TagSourceKind
{
    Virtual,
    Mqtt,
    Http,
    Modbus
}