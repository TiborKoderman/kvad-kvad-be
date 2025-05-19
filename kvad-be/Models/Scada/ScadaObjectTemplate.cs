using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;

public class ScadaObjectTemplate
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    [Column(TypeName = "jsonb")]
    public required JsonObject Data { get; set; } = [];
}