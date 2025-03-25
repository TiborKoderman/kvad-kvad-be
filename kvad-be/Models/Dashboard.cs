using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(Id), nameof(UserId))]
public class Dashboard
{
    public required int Id { get; set; }
    public required User User { get; set; }
    public required Guid UserId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool scrollable { get; set; } = false;
    public string? Icon { get; set; }
    public string? Color { get; set; }
}