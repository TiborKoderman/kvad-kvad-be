using System.Text.Json;
using System.Text.Json.Nodes;

public record ScadaObjectTemplateDTO(
    Guid? Id,
    string Name,
    JsonDocument Data
    );