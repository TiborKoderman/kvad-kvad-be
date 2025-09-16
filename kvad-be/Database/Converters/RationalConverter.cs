using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace kvad_be.Database.Converters;

public class RationalConverter : ValueConverter<Rational, string>
{
    public RationalConverter()
        : base(
            v => SerializeToJson(v),
            v => DeserializeFromJson(v)
        )
    { }

    private static string SerializeToJson(Rational rational)
    {
        var data = new { Numerator = rational.Numerator, Denominator = rational.Denominator };
        return JsonSerializer.Serialize(data);
    }

    private static Rational DeserializeFromJson(string json)
    {
        var data = JsonSerializer.Deserialize<RationalData>(json) 
            ?? throw new InvalidOperationException("Failed to deserialize Rational data from JSON.");
        return new Rational(data.Numerator, data.Denominator);
    }

    private class RationalData
    {
        public long Numerator { get; set; }
        public long Denominator { get; set; }
    }
}
