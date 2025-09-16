using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using NodaTime;

[PrimaryKey(nameof(TagId), nameof(Ts))]
public class TagHist
{
    public required Instant Ts { get; set; }

    [ForeignKey(nameof(Tag))]
    public required int TagId { get; set; }

    [Column(TypeName = "decimal(38, 10)")]
    public decimal? V_decimal { get; set; }
    public double? V_f64 { get; set; }
    public long? V_i64 { get; set; }
    public short? V_enum { get; set; }
    public bool? V_bool { get; set; }
    public string? V_string { get; set; }

    [Column(TypeName = "jsonb")]
    public JsonObject? V_json { get; set; }
    [Column(TypeName = "bytea")]
    public byte[]? V_bytea { get; set; }
    public required TagQuality Quality { get; set; } = TagQuality.Ok;

}