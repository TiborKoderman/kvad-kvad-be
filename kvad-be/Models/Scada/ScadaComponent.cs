using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

public class ScadaComponent
{
    public required string Name { get; set; }
    [Column(TypeName = "jsonb")]
    public required JsonDocument Config { get; set; } = JsonDocument.Parse("{}");
}