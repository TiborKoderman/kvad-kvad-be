using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Dashboard
{
    [Key]
    public required int Id { get; set; }
    [Key, ForeignKey("User")]
    public required Guid UserId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Icon { get; set; }
    public required string Color { get; set; }
}