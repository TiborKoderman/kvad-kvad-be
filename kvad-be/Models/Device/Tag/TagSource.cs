using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

public class TagSource
{
    [Key]
    public required int TagId { get; set; } //one to one with Tag
    [ForeignKey(nameof(TagId))]
    public required Tag Tag { get; set; }
    public required TagSourceKind Kind { get; set; }
    public required TagSourceMeta Meta { get; set; }
    [Column(TypeName = "jsonb")]
    public JsonDocument? Config { get; set; } // json config depending on Kind
}


public enum TagSourceKind
{
    Virtual,
    Mqtt,
    Http,
    Modbus
}

public enum TagSourceMeta
{
    AutoInfered,
    UserDefined,
    SystemDefined,
    DeviceDefined,
    AutoInferedConfirmed
}