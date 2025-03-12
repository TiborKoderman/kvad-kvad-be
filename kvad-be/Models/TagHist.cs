using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Timestamp), nameof(Tag))]
public class TagHist
{
    public required DateTime Timestamp { get; set; }
    public required Tag Tag { get; set; }
    public required JsonValue Value { get; set; }
}