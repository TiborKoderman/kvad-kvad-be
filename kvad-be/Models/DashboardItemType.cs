public class DashboardItemType
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public string? ConfigSchema { get; set; }
    public required List<DashboardItem> DashboardItems { get; set; } = [];
}