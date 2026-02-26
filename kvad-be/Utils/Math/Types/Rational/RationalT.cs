using System.Numerics;

/// <summary>
/// A generic rational number backed by any binary integer type T.
/// Arithmetic uses T-native operations; overflow detection and
/// tier-promotion live in the adaptive <see cref="Rational"/> wrapper.
/// </summary>
public readonly struct Rational<T>
    : IEquatable<Rational<T>>, IComparable<Rational<T>>, IComparable
    where T : IBinaryInteger<T>
{
  // ── Fields ────────────────────────────────────────────────────────────────

  public T Num { get; }
  public T Den { get; }

  // ── Well-known values ─────────────────────────────────────────────────────

  public static readonly Rational<T> Zero = new(T.Zero, T.One);
  public static readonly Rational<T> One = new(T.One, T.One);

  // ── Construction ──────────────────────────────────────────────────────────

  public Rational(T n, T d)
  {
    if (d == T.Zero)
      throw new DivideByZeroException("Denominator cannot be zero.");

    if (n == T.Zero)
    {
      Num = T.Zero;
      Den = T.One;
      return;
    }

    T g = Gcd(T.Abs(n), T.Abs(d));
    n /= g;
    d /= g;

    // Canonical form: denominator is always positive.
    if (d < T.Zero)
    {
      n = T.Zero - n;
      d = T.Zero - d;
    }

    Num = n;
    Den = d;
  }

  // ── Arithmetic operators ──────────────────────────────────────────────────

  public static Rational<T> operator +(Rational<T> a, Rational<T> b)
    => new(a.Num * b.Den + b.Num * a.Den, a.Den * b.Den);

  public static Rational<T> operator -(Rational<T> a, Rational<T> b)
    => new(a.Num * b.Den - b.Num * a.Den, a.Den * b.Den);

  public static Rational<T> operator *(Rational<T> a, Rational<T> b)
    => new(a.Num * b.Num, a.Den * b.Den);

  public static Rational<T> operator /(Rational<T> a, Rational<T> b)
  {
    if (b.Num == T.Zero) throw new DivideByZeroException("Cannot divide by zero rational.");
    return new(a.Num * b.Den, a.Den * b.Num);
  }

  public static Rational<T> operator -(Rational<T> a) => new(T.Zero - a.Num, a.Den);
  public static Rational<T> operator +(Rational<T> a) => a;

  // ── Comparison ────────────────────────────────────────────────────────────

  public int CompareTo(Rational<T> other)
  {
    // (a/b) vs (c/d) → sign(a·d − c·b)  (denominators are always positive)
    T left = Num * other.Den;
    T right = other.Num * Den;
    return T.Sign(left - right);
  }

  int IComparable.CompareTo(object? obj) => obj switch
  {
    null => 1,
    Rational<T> r => CompareTo(r),
    _ => throw new ArgumentException($"Object must be {nameof(Rational<T>)}", nameof(obj))
  };

  // ── Equality ──────────────────────────────────────────────────────────────

  public bool Equals(Rational<T> other) => Num == other.Num && Den == other.Den;

  public override bool Equals(object? obj) => obj is Rational<T> r && Equals(r);

  public override int GetHashCode() => HashCode.Combine(Num, Den);

  // ── Relational operators ──────────────────────────────────────────────────

  public static bool operator ==(Rational<T> a, Rational<T> b) => a.Equals(b);
  public static bool operator !=(Rational<T> a, Rational<T> b) => !a.Equals(b);
  public static bool operator <(Rational<T> a, Rational<T> b) => a.CompareTo(b) < 0;
  public static bool operator >(Rational<T> a, Rational<T> b) => a.CompareTo(b) > 0;
  public static bool operator <=(Rational<T> a, Rational<T> b) => a.CompareTo(b) <= 0;
  public static bool operator >=(Rational<T> a, Rational<T> b) => a.CompareTo(b) >= 0;

  // ── Integer exponentiation ────────────────────────────────────────────────

  /// <summary>Raises x to an integer power. Negative exponents flip numerator/denominator.</summary>
  public static Rational<T> Pow(Rational<T> x, int exp)
  {
    if (exp == 0) return One;
    if (exp < 0) { x = new(x.Den, x.Num); exp = -exp; }
    Rational<T> result = One;
    while (exp > 0)
    {
      if ((exp & 1) == 1) result *= x;
      x *= x;
      exp >>= 1;
    }
    return result;
  }

  // ── Conversions ───────────────────────────────────────────────────────────

  /// <summary>Any T value is a rational with denominator 1.</summary>
  public static implicit operator Rational<T>(T v) => new(v, T.One);

  /// <summary>Truncating integer part.</summary>
  public static explicit operator T(Rational<T> r) => r.Num / r.Den;

  public static explicit operator double(Rational<T> r)
    => double.CreateTruncating(r.Num) / double.CreateTruncating(r.Den);

  public static explicit operator float(Rational<T> r)
    => float.CreateTruncating(r.Num) / float.CreateTruncating(r.Den);

  // ── Helpers ───────────────────────────────────────────────────────────────

  public override string ToString()
    => Den == T.One ? Num.ToString()! : $"{Num}/{Den}";

  private static T Gcd(T a, T b)
  {
    while (b != T.Zero) { T t = b; b = a % b; a = t; }
    return a;
  }

  /// <summary>
  /// Creates a <see cref="Rational{T}"/> that is known to already be in canonical
  /// form (GCD=1, positive denominator). Used internally to skip redundant GCD.
  /// </summary>
  internal static Rational<T> Preformed(T n, T d) => new(n, d, skip: true);

  // Bypass-normalisation constructor – only reachable via Preformed.
  private Rational(T n, T d, bool skip) { Num = n; Den = d; }
}

