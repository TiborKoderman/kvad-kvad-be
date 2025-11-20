using System.ComponentModel.DataAnnotations;


public class Dashboard
{
    [Key]
    public required Guid Id { get; set; }
    public required User Owner { get; set; }
    public List<Group> Groups { get; set; } = [];
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string TypeId { get; set; } //masonry, grid, scada, custom
    public DashboardType DashboardType { get; set; } = null!;
    public bool Scrollable { get; set; } = false;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public Layout? Layout { get; set; }
    public Widget[] Widgets { get; set; } = [];
}