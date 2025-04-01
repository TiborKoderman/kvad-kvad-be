using System.Text.Json.Nodes;

public class WidgetType
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string VueComponent { get; set; }
}