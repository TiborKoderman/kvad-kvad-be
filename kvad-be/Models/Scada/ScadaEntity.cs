using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

public class ScadaEntity
{
    public required Guid Id { get; set; }
    [Column(TypeName = "jsonb")]
    public required ScadaComponent[] Components { get; set; } = Array.Empty<ScadaComponent>();
}