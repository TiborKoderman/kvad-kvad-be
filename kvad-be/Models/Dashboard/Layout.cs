using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(DashboardId), nameof(Id))]
public class Layout
{
    public required Guid DashboardId { get; set; }
    public required Dashboard Dashboard { get; set; }
    public required int Id { get; set; }
    public required enumDirection Direction { get; set; } = enumDirection.row;
    public string? Width { get; set; }
    public string? Height { get; set; }
    public Layout? Parent { get; set; } = null;
    public int? ParentId { get; set; } = null;
    public Layout[]? Children { get; set; } = null;
    public Widget? Widget { get; set; } = null;
}

public enum enumDirection{
    row,
    col,
}