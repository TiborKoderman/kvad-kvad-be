using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

[Keyless]
public class TagCurr
{
    public required DateTime Timestamp { get; set; }
    public required Tag Tag { get; set; }
    public required JsonValue Value { get; set; }
}