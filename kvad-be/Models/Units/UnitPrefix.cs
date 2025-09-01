using System.ComponentModel.DataAnnotations;

public class UnitPrefix
{
    [Key]
    public required int Id { get; set; }
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public required short Exponent { get; set; }
    public required short Power { get; set; }
    public decimal Factor { get; private set; }
}