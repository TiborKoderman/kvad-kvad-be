using System.Numerics;

namespace Utils.Math.Types
{
    public readonly struct Rational
    {
        public BigInteger Numerator { get; }
        public BigInteger Denominator { get; }

        public static readonly Rational One = new(1, 1);
        private static readonly Rational Zero = new(0, 1);

        public Rational(BigInteger numerator, BigInteger denominator)
        {
            if (denominator == 0)
                throw new DivideByZeroException("Denominator cannot be zero.");
            Numerator = numerator;
            Denominator = denominator;
        }

        public static implicit operator Rational(int v) => new(v, 1);
        public static implicit operator Rational(double v) => new((BigInteger)v, 1);
        public static implicit operator Rational(BigInteger v) => new(v, 1);

        public Rational Normalize()
        {
            if (Denominator < 0) return new Rational(-Numerator, -Denominator).Normalize();
            if (Numerator == 0) return new Rational(0, 1);
            var g = BigInteger.GreatestCommonDivisor(BigInteger.Abs(Numerator), BigInteger.Abs(Denominator));
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

        public override string ToString() => $"{Numerator}/{Denominator}";
    }
}
