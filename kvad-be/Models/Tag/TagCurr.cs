using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(DeviceId), nameof(TagId))]
public class TagCurr
{
    public required Guid DeviceId { get; set; }
    public required string TagId { get; set; }
    
    [ForeignKey("DeviceId,TagId")]
    public required Tag Tag { get; set; }
    public required DateTime Timestamp { get; set; }

    [Column(TypeName = "jsonb")]
    public required JsonValue Value { get; set; }
}