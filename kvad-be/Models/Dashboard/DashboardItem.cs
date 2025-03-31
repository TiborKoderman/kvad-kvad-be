public class DashboardItem
{
    public required Guid Id { get; set; }
    public required Dashboard Dashboard { get; set; }
    public Type? Type { get; set; }
    public required DashboardItem?[] Children { get; set; } = [];
}