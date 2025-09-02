using System.ComponentModel.DataAnnotations;

public class UnitPrefix
{
    [Key]
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public required short Base { get; set; } = 10;
    public required short Exponent { get; set; }
}