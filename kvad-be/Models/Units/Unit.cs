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
            Dimension = [0, 0, 0, 0, 0, 0, 0],
            Factor = new Rational(1, 1) // Set default Factor value
        };
    }
    public static Unit CreateUnit(string Symbol, string Name, string Quantity, short[] Dimension, string? Definition, bool Prefixable = true)
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


    public static Unit CreateUnit(string Symbol, string Definition, string? Name)
    {
        // TODO: Implement unit creation from definition string
        // This would parse the definition and create appropriate unit type
        throw new NotImplementedException("Unit creation from definition not yet implemented");
    }

    public static Dictionary<string, Rational> ParseDefinition(string definition)
    {
        var result = new Dictionary<string, Rational>();

        // Split by '/' to handle division - first part is numerator, all others are denominators
        var parts = definition.Split('/');

        // Parse numerator (positive exponents)
        ParseSection(parts[0], result, new Rational(1, 1));

        // Parse all denominators (negative exponents)
        for (int i = 1; i < parts.Length; i++)
        {
            ParseSection(parts[i], result, new Rational(-1, 1));
        }

        return result;
    }
    
    private static void ParseSection(string section, Dictionary<string, Rational> result, Rational signMultiplier)
    {
        // Remove parentheses and normalize spaces
        section = section.Replace("(", "").Replace(")", "").Replace("*", " ");
        
        // Regex to match: number OR unit_symbol, optionally followed by ^exponent
        // Matches: "1000", "kg", "m^2", "s^-1", "42^3", etc.
        var matches = System.Text.RegularExpressions.Regex.Matches(section, @"([a-zA-Z]+|\d+(?:\.\d+)?)(?:\^(-?\d+))?");
        
        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var symbol = match.Groups[1].Value;
            var exponentStr = match.Groups[2].Value;
            
            // Default exponent is 1 if not specified
            var exponent = string.IsNullOrEmpty(exponentStr) ? new Rational(1, 1) : new Rational(int.Parse(exponentStr), 1);
            
            // Apply sign multiplier (for denominator terms)
            exponent = exponent * signMultiplier;
            
            // Add or accumulate exponents for the same symbol
            if (result.ContainsKey(symbol))
                result[symbol] = (result[symbol] + exponent).Normalize();
            else
                result[symbol] = exponent.Normalize();
        }
    }
}

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


