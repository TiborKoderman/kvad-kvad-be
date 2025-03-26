using System.Text.Json;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Id), nameof(DashboardId))]
public class DashboardItem
{
    public int Id { get; set; }
    public required Dashboard Dashboard { get; set; }
    public required int DashboardId { get; set; }
    public required string Title { get; set; }
    public required string Type { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public JsonDocument? Config { get; set; }
    public required List<Tag> Tags { get; set; } = [];
}