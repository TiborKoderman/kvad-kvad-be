using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using kvad_be.Extensions.PostgresComposite;

[PgCompositeType("rational")]
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


    public sealed class BytesConverter : ValueConverter<Rational, byte[]>
    {
        public BytesConverter() : base(
            v => ToBytes(v),
            v => FromBytes(v)
        )
        { }
        public static byte[] ToBytes(Rational v)
        {
            var b = new byte[16];
            System.Buffers.Binary.BinaryPrimitives.WriteInt64LittleEndian(b.AsSpan(0, 8), v.Numerator);
            System.Buffers.Binary.BinaryPrimitives.WriteInt64LittleEndian(b.AsSpan(8, 8), v.Denominator);
            return b;
        }

        public static Rational FromBytes(byte[] b)
        {
            var num = System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(b.AsSpan(0, 8));
            var den = System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(b.AsSpan(8, 8));
            if (den == 0) throw new DivideByZeroException();
            return new(num, den);
        }
    }

}
