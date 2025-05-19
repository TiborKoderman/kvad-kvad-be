using System.Text.Json.Nodes;

public record ScadaObjectTemplateDTO(
    Guid? Id,
    string Name,
    JsonObject Data
    );