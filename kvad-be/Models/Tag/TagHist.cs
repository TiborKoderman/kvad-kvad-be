using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NodaTime;

[PrimaryKey(nameof(SeriesId), nameof(Timestamp))]
public class TagHist
{
    [ForeignKey(nameof(Tag))]
    public required int SeriesId { get; set; }
    public required Instant Timestamp { get; set; }

    public float? v_float { get; set; }
    public double? v_double { get; set; }
    public long? v_int { get; set; }
    public short? v_enum { get; set; }
    public bool? v_bool { get; set; }
    public string? v_string { get; set; }
    [Column(TypeName = "jsonb")]
    public JsonObject? v_json { get; set; }
}