using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace kvad_be.Database.Converters;

public class Dim7Converter : ValueConverter<Dim7, string>
{
    public Dim7Converter()
        : base(
            v => SerializeToJson(v),
            v => DeserializeFromJson(v)
        )
    { }

    private static string SerializeToJson(Dim7 dim7)
    {
        var data = new { L = dim7.L, M = dim7.M, T = dim7.T, I = dim7.I, Th = dim7.Th, N = dim7.N, J = dim7.J };
        return JsonSerializer.Serialize(data);
    }

    private static Dim7 DeserializeFromJson(string json)
    {
        var data = JsonSerializer.Deserialize<Dim7Data>(json) 
            ?? throw new InvalidOperationException("Failed to deserialize Dim7 data from JSON.");
        return new Dim7(data.L, data.M, data.T, data.I, data.Th, data.N, data.J);
    }

    private class Dim7Data
    {
        public short L { get; set; }
        public short M { get; set; }
        public short T { get; set; }
        public short I { get; set; }
        public short Th { get; set; }
        public short N { get; set; }
        public short J { get; set; }
    }
}
