using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;

public class ScadaEntityTemplate
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    [Column(TypeName = "jsonb")]
    public required JsonDocument Data { get; set; } = JsonDocument.Parse("{}");
}