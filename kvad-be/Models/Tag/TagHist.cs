using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(TagDeviceId), nameof(TagId), nameof(Timestamp))]
public class TagHist
{
    public required Guid TagDeviceId { get; set; }
    public required string TagId { get; set; } 
    public required Tag Tag { get; set; }
    public required DateTime Timestamp { get; set; }
    
    [Column(TypeName = "jsonb")]
    public required JsonValue Value { get; set; }
}