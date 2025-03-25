using System.ComponentModel.DataAnnotations;

public class Dashboard
{
    [Key]
    public required Guid Id { get; set; }
    public required User Owner { get; set; }
    public required Group[] Groups { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool scrollable { get; set; } = false;
    public string? Icon { get; set; }
    public string? Color { get; set; }

    public required List<DashboardItem> Items { get; set; } = [];
}