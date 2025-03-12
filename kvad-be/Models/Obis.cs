using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(A), nameof(B), nameof(C), nameof(D), nameof(E), nameof(F))]
public class Obis
{
    public string Id => $"{A}-{B}:{C}.{D}.{E}*{F}";
    //Medium
    public int A { get; set; }
    //Channel
    public int B { get; set; }
    //Physical unit
    public int C { get; set; }
    //Measurement type
    public int D { get; set; }
    //Tariff
    public int E { get; set; }
    //Seperate values defined by A-E
    public int F { get; set; } = 255;

    public string? Description { get; set; }

    public Obis(){}

    public Obis(int a, int b, int c, int d, int e, int? f, string? description = null)
    {
        A = a;
        B = b;
        C = c;
        D = d;
        E = e;
        F = f ?? 255;
        Description = description;
    }

    public static Obis FromId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Id cannot be null or empty.");

        var match = Regex.Match(id, @"^(\d+)-(\d+):(\d+)\.(\d+)\.(\d+)(?:\*(\d+))?$");
        if (!match.Success)
            throw new FormatException($"Invalid Id format: {id}");

        return new Obis(
            int.Parse(match.Groups[1].Value),
            int.Parse(match.Groups[2].Value),
            int.Parse(match.Groups[3].Value),
            int.Parse(match.Groups[4].Value),
            int.Parse(match.Groups[5].Value),
            match.Groups[6].Success ? int.Parse(match.Groups[6].Value) : (int?)null
        );
    }
}