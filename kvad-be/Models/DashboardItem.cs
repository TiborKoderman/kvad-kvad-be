using System.ComponentModel.DataAnnotations;

public class DashboardItem {
    [Key]
    public int Id { get; set; }
    [Key]
    public required Dashboard Dashboard { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public required string Color { get; set; }
}