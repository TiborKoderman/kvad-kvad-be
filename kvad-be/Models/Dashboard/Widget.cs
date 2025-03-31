public class Widget
{
    public required string Id { get; set; }
    public required Dashboard Dashboard { get; set; }
    public required Guid DashboardId { get; set; }
    public required WidgetType Type { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public required string Color { get; set; }
}