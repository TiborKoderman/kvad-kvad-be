using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using NodaTime;

[PrimaryKey(nameof(TagId))]
public class TagCurr
{
    public required string TagId { get; set; }

    [ForeignKey(nameof(TagId))]
    public required Tag Tag { get; set; }
    public required Instant Timestamp { get; set; }

    [Column(TypeName = "jsonb")]
    public required JsonValue Value { get; set; }
    public required double Delta { get; set; } // difference from previous value, if numeric otherwie 1 if changed, 0 if not changed
    public required TagQuality Quality { get; set; }
}