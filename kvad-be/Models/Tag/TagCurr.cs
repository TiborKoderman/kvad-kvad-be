using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

public class TagCurr
{
    public required DateTime Timestamp { get; set; }

    public required int TagId { get; set; }

    public required Tag Tag { get; set; }

    [Column(TypeName = "jsonb")]
    public required JsonValue Value { get; set; }
}