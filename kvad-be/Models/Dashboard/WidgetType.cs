using System.Text.Json.Nodes;

public class WidgetType
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string VueComponent { get; set; }
    // public required JsonNode Config { get; set; } = new JsonObject();
    // public required JsonNode Params { get; set; } = new JsonObject();

}