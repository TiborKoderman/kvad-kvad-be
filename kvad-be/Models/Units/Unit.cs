using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public abstract class Unit
{
    [Key]
    public required string Symbol { get; set; }
    public required string Name { get; set; }
    public string Quantity { get; set; } = "";

    public short[] Dimension { get; set; } = new short[7];
    public bool Prefixable { get; set; } = true;
    public string? Definition { get; set; } = null;

    public required Rational Factor { get; set; } = new(1, 1);

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
    public required Rational LogK { get; set; } = new(1, 1);

    public required Rational LogRef { get; set; } = new(1, 1);
    public required string LogBase { get; set; } = "";
}


