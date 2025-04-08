using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Nodes;

public class Dashboard
{
    [Key]
    public required Guid Id { get; set; }
    public required User Owner { get; set; }
    public List<Group> Groups { get; set; } = [];
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; } //masonry, grid, scada, custom
    public bool Scrollable { get; set; } = false;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public Layout? Layout { get; set; }
    public Widget[] Widgets { get; set; } = [];
}