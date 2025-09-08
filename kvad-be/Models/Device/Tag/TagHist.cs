using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using NodaTime;

[PrimaryKey(nameof(SeriesId), nameof(Timestamp))]
public class TagHist
{
    public required Instant Timestamp { get; set; }

    [ForeignKey(nameof(Tag))]
    public required long SeriesId { get; set; }

    public double? V_f64 { get; set; }
    public long? V_i64 { get; set; }
    public short? V_enum { get; set; }
    [Column(TypeName = "decimal(38, 10)")]
    public decimal? V_decimal { get; set; }
    public bool? V_bool { get; set; }
    public string? V_string { get; set; }

    [Column(TypeName = "jsonb")]
    public JsonObject? V_json { get; set; }
    [Column(TypeName = "bytea")]
    public byte[]? V_bytea { get; set; }
    public required TagQuality Quality { get; set; }

}