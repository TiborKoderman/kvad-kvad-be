using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public interface IUnitFactory
{
    Unit CreateUnit(string symbol, string name, string? definition, string quantity, Dim7 dimension);
}

public abstract class Unit
{
    [Key]
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public string Quantity { get; set; } = "";
    public Dim7 Dimension { get; set; } = new Dim7();
    public bool Prefixable { get; set; } = true;
    public string? Definition { get; set; } = null;

    [InverseProperty(nameof(UnitCanonicalPart.Unit))]
    public UnitCanonicalPart[] CanonicalParts { get; set; } = [];

    [InverseProperty(nameof(UnitCanonicalPart.Part))]
    public UnitCanonicalPart[] PartOfUnits { get; set; } = [];

    public static Unit CreateUnit(string Symbol, string Name, string? Definition, string Quantity, Dim7 Dimension)
    {
        return new LinearUnit
        {
            Symbol = Symbol,
            Name = Name,
            Definition = Definition,
            Quantity = Quantity,
            Dimension = Dimension,
            Factor = new Rational(1, 1) // Set default Factor value
        };
    }
}

public class LinearUnit : Unit
{
    public required Rational Factor { get; set; } = Rational.One;
}

public class AffineUnit : Unit
{
    public required Rational Factor { get; set; } = Rational.One;
    public required decimal Offset { get; set; }
}

public class LogarithmicUnit : Unit
{
    public required Rational LogK { get; set; } = Rational.One;
    public required Rational LogRef { get; set; } = Rational.One;
    public required string LogBase { get; set; } = "";
}


