using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace kvad_be.Database.Converters;

/// <summary>
/// Value converter for Rational type that works with PostgreSQL composite types
/// This converter handles the translation between Rational objects and a simple struct format
/// </summary>
public class RationalCompositeConverter : ValueConverter<Rational, RationalData>
{
    public RationalCompositeConverter()
        : base(
            v => new RationalData { Numerator = v.Numerator, Denominator = v.Denominator },
            v => new Rational(v.Numerator, v.Denominator)
        )
    { }
}

public struct RationalData
{
    public long Numerator { get; set; }
    public long Denominator { get; set; }
}
