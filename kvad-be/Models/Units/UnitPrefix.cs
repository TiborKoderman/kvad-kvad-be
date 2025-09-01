using System.ComponentModel.DataAnnotations;
using Utils.Math.Types;

public class UnitPrefix
{
    [Key]
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public required short Base { get; set; } = 10;
    public required short Exponent { get; set; }
    public Rational Factor() {
        return Rational.Pow(new Rational(Base, 1), Exponent);
    }
}