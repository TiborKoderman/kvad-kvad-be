using System.Numerics;


public struct Rational32 : IEquatable<Rational32>, IComparable<Rational32>, IComparable
{
  private Int32 Num { get; set; }
  private Int32 Den { get; set; }

  public static readonly Rational32 One = new(1, 1);
  public static readonly Rational32 Zero = new(0, 1);

  public Rational32(Int32 numerator, Int32 denominator)
  {
    Num = numerator;
    Den = denominator;
    Normalize();
  }

  public static Rational32 operator *(Rational32 a, Rational32 b)
  {
    if (a.Num * b.Num > Int32.MaxValue || a.Num * b.Num < Int32.MinValue)
      throw new OverflowException("Numerator overflow.");
    if (a.Den * b.Den > Int32.MaxValue || a.Den * b.Den < Int32.MinValue)
      throw new OverflowException("Denominator overflow.");

    return new Rational32(a.Num * b.Num, a.Den * b.Den);
  }

  public static Rational32 operator /(Rational32 a, Rational32 b)
  {
    if (a.Num * b.Den > Int32.MaxValue || a.Num * b.Den < Int32.MinValue)
      throw new OverflowException("Numerator overflow.");
    if (a.Den * b.Num > Int32.MaxValue || a.Den * b.Num < Int32.MinValue)
      throw new OverflowException("Denominator overflow.");

    return new Rational32(a.Num * b.Den, a.Den * b.Num);
  }

  public static Rational32 operator +(Rational32 a, Rational32 b)
  {
    if (a.Num * b.Den > Int32.MaxValue || a.Num * b.Den < Int32.MinValue)
      throw new OverflowException("Numerator overflow.");
    if (b.Num * a.Den > Int32.MaxValue || b.Num * a.Den < Int32.MinValue)
      throw new OverflowException("Numerator overflow.");
    if (a.Den * b.Den > Int32.MaxValue || a.Den * b.Den < Int32.MinValue)
      throw new OverflowException("Denominator overflow.");

    return new Rational32(a.Num * b.Den + b.Num * a.Den, a.Den * b.Den);
  }

  public static Rational32 operator -(Rational32 a, Rational32 b)
  {
    if (a.Num * b.Den > Int32.MaxValue || a.Num * b.Den < Int32.MinValue)
      throw new OverflowException("Numerator overflow.");
    if (b.Num * a.Den > Int32.MaxValue || b.Num * a.Den < Int32.MinValue)
      throw new OverflowException("Numerator overflow.");
    if (a.Den * b.Den > Int32.MaxValue || a.Den * b.Den < Int32.MinValue)
      throw new OverflowException("Denominator overflow.");

    return new Rational32(a.Num * b.Den - b.Num * a.Den, a.Den * b.Den);
  }

  public static Boolean operator ==(Rational32 a, Rational32 b)
  {
    return Equals(a, b);
  }

  public static Boolean operator !=(Rational32 a, Rational32 b)
  {
    return !Equals(a, b);
  }

  public static Boolean operator <(Rational32 a, Rational32 b) => a.CompareTo(b) < 0;
  public static Boolean operator >(Rational32 a, Rational32 b) => a.CompareTo(b) > 0;
  public static Boolean operator <=(Rational32 a, Rational32 b) => a.CompareTo(b) <= 0;
  public static Boolean operator >=(Rational32 a, Rational32 b) => a.CompareTo(b) >= 0;

  public readonly Int32 CompareTo(Rational32 other)
  {
    // Compare a/b vs c/d by comparing a*d and c*b in 64-bit.
    Int64 left = (Int64)Num * other.Den;
    Int64 right = (Int64)other.Num * Den;
    return left.CompareTo(right);
  }

  // Object-based compare: used when someone only knows "IComparable".
  readonly Int32 IComparable.CompareTo(Object? obj)
  {
    if (obj is null) return 1;                 // any value > null by convention
    if (obj is Rational32 r) return CompareTo(r);

    // IMPORTANT: implicit conversions do NOT apply here.
    throw new ArgumentException(
        $"Object must be of type {nameof(Rational32)}",
        nameof(obj));
  }



  public static Rational32 Pow(Rational32 mantissa, Int32 e)
  {
    if (e == 0)
      return One;
    if (e < 0)
    {
      mantissa = new Rational32(mantissa.Den, mantissa.Num);
      e = -e;
    }
    Rational32 result = One;
    while (e > 0)
    {
      if ((e & 1) == 1)
        result *= mantissa;
      mantissa *= mantissa;
      e >>= 1;
    }
    return result;
  }

  public static Double Pow(Rational32 mantissa, Rational32 e)
  {
    //if exponent is an integer, we can use the Rational32 Pow method
    if (e.Den == 1)
    {
      return (Double)Pow(mantissa, e.Num);
    }
    //otherwise, we need to convert to double and use Math.Pow
    return Math.Pow((Double)mantissa, (Double)e);
  }

  public readonly override Boolean Equals(Object? obj)
  {
    if (obj is Rational32 r)
      return Num == r.Num && Den == r.Den;
    return false;
  }

  public readonly Boolean Equals(Rational32 other)
  {
    return Num == other.Num && Den == other.Den;
  }

  public readonly override Int32 GetHashCode()
  {
    return HashCode.Combine(Num, Den);
  }

  public readonly override String ToString()
  {
    return $"{Num}/{Den}";
  }

  private void Normalize()
  {
    if (Den == 0)
      throw new DivideByZeroException("Denominator cannot be zero.");
    if (Den < 0)
    {
      Num = -Num;
      Den = -Den;
    }
    if (Num == 0)
    {
      Den = 1;
      return;
    }
    Int32 g = Gcd(Num, Den);
    Num /= g;
    Den /= g;
  }

  private static Int32 Gcd(Int32 a, Int32 b)
  {
    Int64 x = a;
    Int64 y = b;
    x = Math.Abs(x);
    y = Math.Abs(y);
    while (y != 0)
    {
      Int64 t = x % y;
      x = y;
      y = t;
    }
    if (x > Int32.MaxValue) throw new OverflowException("GCD overflow"); // rare
    return (Int32)x;
  }


  public static implicit operator Rational32(Int32 v) => new(v, 1);
  public static implicit operator Rational32(Double v) => new((Int32)v, 1);
  public static implicit operator Rational32(BigInteger v) => new((Int32)v, 1);
  public static implicit operator Rational32(Decimal v) => new((Int32)v, 1);

  public static implicit operator Int32(Rational32 r) => r.Num / r.Den;
  public static implicit operator Double(Rational32 r) => (Double)r.Num / r.Den;
  public static implicit operator Decimal(Rational32 r) => (Decimal)r.Num / r.Den;
}
