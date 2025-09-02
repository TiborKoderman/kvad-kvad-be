using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public interface IUnitFactory
{
    public static Unit CreateUnit(string Symbol, string Name, string Quantity, string? Definition)
    {
        return new LinearUnit
        {
            Symbol = Symbol,
            Name = Name,
            Definition = Definition,
            Quantity = Quantity,
            Dimension = new Dim7(0, 0, 0, 0, 0, 0, 0),
            Factor = new Rational(1, 1) // Set default Factor value
        };
    }
    public static Unit CreateUnit(string Symbol, string Name, string Quantity, Dim7 Dimension, string? Definition, bool Prefixable = true)
    {
        return new LinearUnit
        {
            Symbol = Symbol,
            Name = Name,
            Definition = Definition,
            Quantity = Quantity,
            Dimension = Dimension,
            Prefixable = Prefixable,
            Factor = new Rational(1, 1) // Set default Factor value
        };
    }
}

public abstract class Unit
{
    [Key]
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public string Quantity { get; set; } = "";
    
    [NotMapped] // Temporarily ignore for design-time compatibility
    public Dim7 Dimension { get; set; } = Dim7.Zero;
    public bool Prefixable { get; set; } = true;
    public string? Definition { get; set; } = null;
    
    [NotMapped] // Temporarily ignore for design-time compatibility
    public required Rational Factor { get; set; } = Rational.One;

    [InverseProperty(nameof(UnitCanonicalPart.Unit))]
    public UnitCanonicalPart[] CanonicalParts { get; set; } = [];

    [InverseProperty(nameof(UnitCanonicalPart.Part))]
    public UnitCanonicalPart[] PartOfUnits { get; set; } = [];


}


public class LinearUnit : Unit
{
}

public class AffineUnit : Unit
{
    public required decimal Offset { get; set; }
}

public class LogarithmicUnit : Unit
{
    [NotMapped] // Temporarily ignore for design-time compatibility
    public required Rational LogK { get; set; } = Rational.One;
    
    [NotMapped] // Temporarily ignore for design-time compatibility
    public required Rational LogRef { get; set; } = Rational.One;
    public required string LogBase { get; set; } = "";
}


