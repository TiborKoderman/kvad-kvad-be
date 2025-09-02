using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public readonly struct Rational
{
    // Auto-inferred: field name = "Numerator", type = "BIGINT"
    public long Numerator { get; }

    // Auto-inferred: field name = "Denominator", type = "BIGINT"
    public long Denominator { get; }

    public static readonly Rational One = new(1, 1);
    private static readonly Rational Zero = new(0, 1);

    public Rational(long numerator, long denominator)
    {
        if (denominator == 0)
            throw new DivideByZeroException("Denominator cannot be zero.");
        Numerator = numerator;
        Denominator = denominator;
    }

    public Rational(BigInteger numerator, BigInteger denominator)
    {
        if (denominator.IsZero)
            throw new DivideByZeroException("Denominator cannot be zero.");
        if (numerator < long.MinValue || numerator > long.MaxValue)
            throw new OverflowException("Numerator is out of range for long.");
        if (denominator < long.MinValue || denominator > long.MaxValue)
            throw new OverflowException("Denominator is out of range for long.");
        Numerator = (long)numerator;
        Denominator = (long)denominator;
    }



    public static implicit operator Rational(int v) => new(v, 1);
    public static implicit operator Rational(double v) => new((long)v, 1);
    public static implicit operator Rational(BigInteger v) => new((long)v, 1);
    public static implicit operator Rational(decimal v) => new((long)v, 1);

    public Rational Normalize()
    {
        if (Denominator < 0) return new Rational(-Numerator, -Denominator).Normalize();
        if (Numerator == 0) return new Rational(0, 1);
        long g = (long)BigInteger.GreatestCommonDivisor(BigInteger.Abs(Numerator), BigInteger.Abs(Denominator));
        return new Rational(Numerator / g, Denominator / g);
    }

    public static Rational operator *(Rational a, Rational b)
        => new Rational(a.Numerator * b.Numerator, a.Denominator * b.Denominator).Normalize();

    public static Rational operator /(Rational a, Rational b)
        => new Rational(a.Numerator * b.Denominator, a.Denominator * b.Numerator).Normalize();

    public decimal ToDecimal()
        => (decimal)Numerator / (decimal)Denominator;

    public static Rational Pow(Rational a, int n)
    {
        if (n == 0) return One;
        if (n < 0) return Pow(new Rational(a.Denominator, a.Numerator), -n);
        return new Rational(BigInteger.Pow(a.Numerator, n), BigInteger.Pow(a.Denominator, n)).Normalize();
    }

    //value compare


    public override string ToString() => $"{Numerator}/{Denominator}";


    public sealed class LongArrayConverter : ValueConverter<Rational, long[]>
    {
        public LongArrayConverter() : base(
            v => ToLongArray(v),
            v => FromLongArray(v)
        )
        { }

        public static long[] ToLongArray(Rational v)
        {
            return new long[] { v.Numerator, v.Denominator };
        }

        public static Rational FromLongArray(long[] arr)
        {
            if (arr == null || arr.Length != 2)
                throw new ArgumentException("Array must contain exactly two elements.");
            if (arr[1] == 0)
                throw new DivideByZeroException();
            return new Rational(arr[0], arr[1]);
        }
    }

    public sealed class SerialConverter: ValueConverter<Rational, string>
    {
        public SerialConverter() : base(
            v => ToString(v),
            v => FromString(v))
        { }

        public static string ToString(Rational r)
        {
            return "ROW(" + r.Numerator + ", " + r.Denominator + ")";
        }

        public static Rational FromString(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentException("Input string cannot be null or empty.", nameof(s));

            var parts = s.Split('/');
            if (parts.Length != 2)
                throw new FormatException("Input string must be in the format 'numerator/denominator'.");

            if (!long.TryParse(parts[0], out long numerator))
                throw new FormatException("Numerator is not a valid long integer.");

            if (!long.TryParse(parts[1], out long denominator))
                throw new FormatException("Denominator is not a valid long integer.");

            if (denominator == 0)
                throw new DivideByZeroException("Denominator cannot be zero.");

            return new Rational(numerator, denominator);
        }
    }
}


