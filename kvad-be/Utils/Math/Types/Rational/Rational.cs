using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// An adaptive rational number that transparently upgrades its internal integer
/// storage (32-bit → 64-bit → 128-bit → arbitrary precision) when arithmetic
/// would exceed the current capacity.
///
/// For the overwhelming common case (values that fit in 32-bit or 64-bit integers)
/// no heap allocation occurs. The 128-bit and BigInteger tiers box a single small
/// object only when genuinely needed.
/// </summary>
public readonly struct Rational : IEquatable<Rational>, IComparable<Rational>, IComparable
{
  // ── Storage tier ──────────────────────────────────────────────────────────
  //
  //  K32  – numerator/denominator fit in Int32, stored in _num/_den (as long).
  //  K64  – numerator/denominator fit in Int64, stored in _num/_den.
  //  K128 – _big holds a boxed Rational<Int128>.
  //  KBig – _big holds a boxed Rational<BigInteger>.
  //
  private enum Kind : byte { K32, K64, K128, KBig }

  private readonly long _num;
  private readonly long _den;
  private readonly object? _big;   // Rational<Int128> | Rational<BigInteger>
  private readonly Kind _kind;

  // ── Public constructors ──────────────────────────────────────────────────

  /// <summary>Creates a rational from a 32-bit numerator and denominator, normalising and
  /// upgrading the storage tier automatically if needed.</summary>
  public Rational(int n, int d)
  {
    var r = FromInt64((long)n, (long)d);
    _num = r._num; _den = r._den; _kind = r._kind; _big = r._big;
  }

  /// <summary>Creates a rational from a 64-bit numerator and denominator.</summary>
  public Rational(long n, long d)
  {
    // Route through Int128 so edge cases like long.MinValue are handled safely.
    var r = FromInt128((Int128)n, (Int128)d);
    _num = r._num; _den = r._den; _kind = r._kind; _big = r._big;
  }

  /// <summary>Creates a rational from a 128-bit numerator and denominator.</summary>
  public Rational(Int128 n, Int128 d)
  {
    var r = FromInt128(n, d);
    _num = r._num; _den = r._den; _kind = r._kind; _big = r._big;
  }

  /// <summary>Creates a rational from arbitrary-precision numerator and denominator.</summary>
  public Rational(BigInteger n, BigInteger d)
  {
    var r = FromBigInteger(n, d);
    _num = r._num; _den = r._den; _kind = r._kind; _big = r._big;
  }

  // ── Private constructors (raw, pre-normalised storage) ───────────────────

  // The bool parameter is only a disambiguator – its value is ignored.
  private Rational(int num, int den, bool _)
  { _num = num; _den = den; _kind = Kind.K32; _big = null; }

  private Rational(long num, long den, bool _)
  { _num = num; _den = den; _kind = Kind.K64; _big = null; }

  private Rational(Rational<Int128> r)
  { _num = 0; _den = 1; _kind = Kind.K128; _big = r; }

  private Rational(Rational<BigInteger> r)
  { _num = 0; _den = 1; _kind = Kind.KBig; _big = r; }

  // ── Well-known values ─────────────────────────────────────────────────────

  public static readonly Rational Zero = new(0, 1, true);
  public static readonly Rational One = new(1, 1, true);

  // ── Diagnostics ───────────────────────────────────────────────────────────

  /// <summary>Human-readable label of the current storage tier.</summary>
  public string StorageTier => _kind switch
  {
    Kind.K32 => "int32",
    Kind.K64 => "int64",
    Kind.K128 => "int128",
    _ => "biginteger"
  };

  // ── Implicit construction ──────────────────────────────────────────────────

  public static implicit operator Rational(int v) => new(v, 1, true);  // v/1 is always canonical

  public static implicit operator Rational(long v)
    => (v >= int.MinValue && v <= int.MaxValue) ? new((int)v, 1, true) : new(v, 1L, true);

  public static implicit operator Rational(Int128 v)
    => FromInt128(v, Int128.One);

  public static implicit operator Rational(BigInteger v)
    => FromBigInteger(v, BigInteger.One);

  // Conversion FROM typed generics (values are already normalised).
  public static implicit operator Rational(Rational<int> r) => new((int)r.Num, (int)r.Den, true);
  public static implicit operator Rational(Rational<long> r) => DowncastInt64(r.Num, r.Den);
  public static implicit operator Rational(Rational<Int128> r) => DowncastInt128(r.Num, r.Den);
  public static implicit operator Rational(Rational<BigInteger> r) => DowncastBig(r.Num, r.Den);

  // ── Explicit extraction ───────────────────────────────────────────────────

  public static explicit operator int(Rational r)
  {
    if (r._kind == Kind.K32) return checked((int)r._num / (int)r._den);
    var (n, d) = r.GetBig(); return checked((int)(n / d));
  }

  public static explicit operator long(Rational r)
  {
    if (r._kind <= Kind.K64) return _num64(r) / _den64(r);
    var (n, d) = r.GetBig(); return checked((long)(n / d));
  }

  public static explicit operator Int128(Rational r)
  {
    if (r._kind <= Kind.K64)
    {
      long n = _num64(r), dv = _den64(r);
      return (Int128)n / (Int128)dv;
    }
    var (bn, bd) = r.GetBig(); return checked((Int128)(bn / bd));
  }

  public static explicit operator BigInteger(Rational r)
  {
    var (n, d) = r.GetBig(); return n / d;
  }

  public static explicit operator double(Rational r)
  {
    var (n, d) = r.GetBig(); return (double)n / (double)d;
  }

  public static explicit operator float(Rational r)
  {
    var (n, d) = r.GetBig(); return (float)n / (float)d;
  }

  // Upcast to a specific generic tier (throws if value doesn't fit).
  public static explicit operator Rational<int>(Rational r)
  {
    if (r._kind != Kind.K32) throw new InvalidCastException("Value does not fit in Rational<int>.");
    return Rational<int>.Preformed((int)r._num, (int)r._den);
  }

  public static explicit operator Rational<long>(Rational r)
  {
    if (r._kind > Kind.K64) throw new InvalidCastException("Value does not fit in Rational<long>.");
    return Rational<long>.Preformed(_num64(r), _den64(r));
  }

  public static explicit operator Rational<Int128>(Rational r)
  {
    if (r._big is Rational<Int128> ri) return ri;
    if (r._kind <= Kind.K64) return Rational<Int128>.Preformed(_num64(r), _den64(r));
    throw new InvalidCastException("Value does not fit in Rational<Int128>.");
  }

  public static explicit operator Rational<BigInteger>(Rational r)
  {
    if (r._big is Rational<BigInteger> rb) return rb;
    var (n, d) = r.GetBig();
    return Rational<BigInteger>.Preformed(n, d);
  }

  // ── Arithmetic ────────────────────────────────────────────────────────────

  public static Rational operator +(Rational a, Rational b)
  {
    Kind tier = a._kind > b._kind ? a._kind : b._kind;

    if (tier <= Kind.K32)
    {
      long an = (int)a._num, ad = (int)a._den;
      long bn = (int)b._num, bd = (int)b._den;
      return FromInt64(an * bd + bn * ad, ad * bd);
    }
    if (tier == Kind.K64)
    {
      Int128 an = a._num, ad = a._den, bn = b._num, bd = b._den;
      return FromInt128(an * bd + bn * ad, ad * bd);
    }
    {
      var (an, ad) = a.GetBig();
      var (bn, bd) = b.GetBig();
      return FromBigInteger(an * bd + bn * ad, ad * bd);
    }
  }

  public static Rational operator -(Rational a, Rational b)
  {
    Kind tier = a._kind > b._kind ? a._kind : b._kind;

    if (tier <= Kind.K32)
    {
      long an = (int)a._num, ad = (int)a._den;
      long bn = (int)b._num, bd = (int)b._den;
      return FromInt64(an * bd - bn * ad, ad * bd);
    }
    if (tier == Kind.K64)
    {
      Int128 an = a._num, ad = a._den, bn = b._num, bd = b._den;
      return FromInt128(an * bd - bn * ad, ad * bd);
    }
    {
      var (an, ad) = a.GetBig();
      var (bn, bd) = b.GetBig();
      return FromBigInteger(an * bd - bn * ad, ad * bd);
    }
  }

  public static Rational operator *(Rational a, Rational b)
  {
    Kind tier = a._kind > b._kind ? a._kind : b._kind;

    if (tier <= Kind.K32)
    {
      long an = (int)a._num, ad = (int)a._den;
      long bn = (int)b._num, bd = (int)b._den;
      return FromInt64(an * bn, ad * bd);
    }
    if (tier == Kind.K64)
    {
      Int128 an = a._num, ad = a._den, bn = b._num, bd = b._den;
      return FromInt128(an * bn, ad * bd);
    }
    {
      var (an, ad) = a.GetBig();
      var (bn, bd) = b.GetBig();
      return FromBigInteger(an * bn, ad * bd);
    }
  }

  public static Rational operator /(Rational a, Rational b)
  {
    if (b.IsZero) throw new DivideByZeroException("Cannot divide by zero rational.");

    Kind tier = a._kind > b._kind ? a._kind : b._kind;

    if (tier <= Kind.K32)
    {
      long an = (int)a._num, ad = (int)a._den;
      long bn = (int)b._num, bd = (int)b._den;
      return FromInt64(an * bd, ad * bn);   // a/b ÷ c/d = a*d / b*c
    }
    if (tier == Kind.K64)
    {
      Int128 an = a._num, ad = a._den, bn = b._num, bd = b._den;
      return FromInt128(an * bd, ad * bn);
    }
    {
      var (an, ad) = a.GetBig();
      var (bn, bd) = b.GetBig();
      return FromBigInteger(an * bd, ad * bn);
    }
  }

  public static Rational operator -(Rational a)
  {
    // Use one tier up so Int32.MinValue negation never silently wraps.
    if (a._kind <= Kind.K32)
    {
      long n = -(long)(int)a._num, d = (long)(int)a._den;
      if (n >= int.MinValue && n <= int.MaxValue) return new((int)n, (int)d, true);
      return new(n, d, true);   // promoted to K64 (e.g. when a.Num == int.MinValue)
    }
    if (a._kind == Kind.K64)
    {
      // -(Int64.MinValue) overflows Int64 – use Int128 path.
      Int128 n = -(Int128)a._num, d = (Int128)a._den;
      return FromInt128(n, d);
    }
    {
      var (n, d) = a.GetBig();
      return FromBigInteger(-n, d);
    }
  }

  public static Rational operator +(Rational a) => a;

  // ── Integer exponentiation ────────────────────────────────────────────────

  /// <summary>Raises x to an integer power via binary exponentiation.</summary>
  public static Rational Pow(Rational x, int exp)
  {
    if (exp == 0) return One;
    if (exp < 0) { x = One / x; exp = -exp; }
    Rational result = One;
    while (exp > 0)
    {
      if ((exp & 1) == 1) result *= x;
      x *= x;
      exp >>= 1;
    }
    return result;
  }

  // ── Comparison ────────────────────────────────────────────────────────────

  public int CompareTo(Rational other)
  {
    // Fast path: both small – cross-multiply in Int128 (no overflow for Int64 inputs).
    if (_kind <= Kind.K64 && other._kind <= Kind.K64)
    {
      Int128 left = (Int128)_num * other._den;
      Int128 right = (Int128)other._num * _den;
      return Int128.Sign(left - right);
    }
    // General path via BigInteger.
    var (an, ad) = GetBig();
    var (bn, bd) = other.GetBig();
    return BigInteger.Compare(an * bd, bn * ad);
  }

  int IComparable.CompareTo(object? obj) => obj switch
  {
    null => 1,
    Rational r => CompareTo(r),
    _ => throw new ArgumentException($"Object must be {nameof(Rational)}", nameof(obj))
  };

  // ── Equality ──────────────────────────────────────────────────────────────

  public bool Equals(Rational other)
  {
    // Same tier: quick field comparison on canonical forms.
    if (_kind == other._kind)
    {
      return _kind switch
      {
        Kind.K32 or Kind.K64 => _num == other._num && _den == other._den,
        Kind.K128 => ((Rational<Int128>)_big!).Equals((Rational<Int128>)other._big!),
        _ => ((Rational<BigInteger>)_big!).Equals((Rational<BigInteger>)other._big!)
      };
    }
    // Different tiers: fall back to canonical BigInteger comparison.
    var (an, ad) = GetBig();
    var (bn, bd) = other.GetBig();
    return an == bn && ad == bd;
  }

  public override bool Equals(object? obj) => obj is Rational r && Equals(r);

  // Must be tier-independent so equal values from different tiers hash the same.
  public override int GetHashCode()
  {
    var (n, d) = GetBig();
    return HashCode.Combine(n, d);
  }

  // ── Relational operators ──────────────────────────────────────────────────

  public static bool operator ==(Rational a, Rational b) => a.Equals(b);
  public static bool operator !=(Rational a, Rational b) => !a.Equals(b);
  public static bool operator <(Rational a, Rational b) => a.CompareTo(b) < 0;
  public static bool operator >(Rational a, Rational b) => a.CompareTo(b) > 0;
  public static bool operator <=(Rational a, Rational b) => a.CompareTo(b) <= 0;
  public static bool operator >=(Rational a, Rational b) => a.CompareTo(b) >= 0;

  // ── Convenience properties ────────────────────────────────────────────────

  public bool IsZero => _kind <= Kind.K64 ? _num == 0 : GetBig().Item1.IsZero;
  public bool IsPositive => _kind <= Kind.K64 ? _num > 0 : GetBig().Item1.Sign > 0;
  public bool IsNegative => _kind <= Kind.K64 ? _num < 0 : GetBig().Item1.Sign < 0;

  public bool IsInteger => _kind switch
  {
    Kind.K32 or Kind.K64 => _den == 1,
    Kind.K128 => ((Rational<Int128>)_big!).Den == Int128.One,
    _ => ((Rational<BigInteger>)_big!).Den.IsOne
  };

  /// <summary>
  /// Numerator as a 64-bit integer. Throws <see cref="OverflowException"/> when
  /// the stored value does not fit in <see cref="long"/> (K128 or KBig tier).
  /// </summary>
  public long Numerator
  {
    get
    {
      if (_kind <= Kind.K64) return _num;
      return long.CreateChecked(GetBig().num);
    }
  }

  /// <summary>
  /// Denominator as a 64-bit integer. Throws <see cref="OverflowException"/> when
  /// the stored value does not fit in <see cref="long"/> (K128 or KBig tier).
  /// </summary>
  public long Denominator
  {
    get
    {
      if (_kind <= Kind.K64) return _den;
      return long.CreateChecked(GetBig().den);
    }
  }

  /// <summary>
  /// Returns this instance unchanged. The <see cref="Rational"/> type is always
  /// stored in canonical (normalised) form; this method exists for API compatibility.
  /// </summary>
  public Rational Normalize() => this;

  // ── Nested EF Core converter ──────────────────────────────────────────────

  /// <summary>EF Core value converter that persists a <see cref="Rational"/> as a two-element
  /// <c>bigint[]</c> column: <c>[numerator, denominator]</c>.</summary>
  public sealed class LongArrayConverter : ValueConverter<Rational, long[]>
  {
    public LongArrayConverter()
      : base(v => new long[] { v.Numerator, v.Denominator },
             v => new Rational(v[0], v[1]))
    { }
  }

  // ── Formatting ────────────────────────────────────────────────────────────

  public override string ToString() => _kind switch
  {
    Kind.K32 or Kind.K64 => _den == 1 ? _num.ToString() : $"{_num}/{_den}",
    Kind.K128 => ((Rational<Int128>)_big!).ToString(),
    _ => ((Rational<BigInteger>)_big!).ToString()
  };

  // ── Internal helpers: reading values ──────────────────────────────────────

  private static long _num64(Rational r) => r._num;
  private static long _den64(Rational r) => r._den;

  private (BigInteger num, BigInteger den) GetBig()
  {
    switch (_kind)
    {
      case Kind.K32: return ((BigInteger)(int)_num, (BigInteger)(int)_den);
      case Kind.K64: return ((BigInteger)_num, (BigInteger)_den);
      case Kind.K128: { var r = (Rational<Int128>)_big!; return ((BigInteger)r.Num, (BigInteger)r.Den); }
      default: { var r = (Rational<BigInteger>)_big!; return (r.Num, r.Den); }
    }
  }

  // ── Internal helpers: normalise and build ─────────────────────────────────

  // Normalise (n, d) expressed as Int64 and choose the smallest fitting tier.
  // Arithmetic on two K32 inputs is done in Int64 (provably overflow-free).
  private static Rational FromInt64(long n, long d)
  {
    if (d < 0) { n = -n; d = -d; }
    if (n == 0) return new(0, 1, true);
    long g = Gcd64(n < 0 ? -n : n, d);
    n /= g; d /= g;
    if (n >= int.MinValue && n <= int.MaxValue && d <= int.MaxValue)
      return new((int)n, (int)d, true);
    return new(n, d, true);
  }

  // Normalise (n, d) expressed as Int128, downcast as far as possible.
  // Arithmetic on two K64 inputs is done in Int128 (provably overflow-free).
  private static Rational FromInt128(Int128 n, Int128 d)
  {
    if (d < 0) { n = -n; d = -d; }
    if (n == 0) return new(0, 1, true);
    Int128 g = Gcd128(n < 0 ? -n : n, d);
    n /= g; d /= g;
    if (n >= int.MinValue && n <= int.MaxValue && d <= int.MaxValue) return new((int)n, (int)d, true);
    if (n >= long.MinValue && n <= long.MaxValue && d <= long.MaxValue) return new((long)n, (long)d, true);
    return new(Rational<Int128>.Preformed(n, d));
  }

  // Normalise (n, d) expressed as BigInteger, downcast as far as possible.
  private static Rational FromBigInteger(BigInteger n, BigInteger d)
  {
    if (d < 0) { n = -n; d = -d; }
    if (n == 0) return new(0, 1, true);
    BigInteger g = BigInteger.GreatestCommonDivisor(BigInteger.Abs(n), d);
    n /= g; d /= g;
    if (n >= int.MinValue && n <= int.MaxValue && d > 0 && d <= int.MaxValue) return new((int)n, (int)d, true);
    if (n >= long.MinValue && n <= long.MaxValue && d > 0 && d <= long.MaxValue) return new((long)n, (long)d, true);
    if (n >= (BigInteger)Int128.MinValue && n <= (BigInteger)Int128.MaxValue
        && d > 0 && d <= (BigInteger)Int128.MaxValue)
      return new(Rational<Int128>.Preformed((Int128)n, (Int128)d));
    return new(Rational<BigInteger>.Preformed(n, d));
  }

  // Downcast helpers – input is already normalised, skip GCD.
  private static Rational DowncastInt64(long n, long d)
  {
    if (n >= int.MinValue && n <= int.MaxValue && d <= int.MaxValue) return new((int)n, (int)d, true);
    return new(n, d, true);
  }

  private static Rational DowncastInt128(Int128 n, Int128 d)
  {
    if (n >= int.MinValue && n <= int.MaxValue && d <= int.MaxValue) return new((int)n, (int)d, true);
    if (n >= long.MinValue && n <= long.MaxValue && d <= long.MaxValue) return new((long)n, (long)d, true);
    return new(Rational<Int128>.Preformed(n, d));
  }

  private static Rational DowncastBig(BigInteger n, BigInteger d)
  {
    if (n >= int.MinValue && n <= int.MaxValue && d > 0 && d <= int.MaxValue) return new((int)n, (int)d, true);
    if (n >= long.MinValue && n <= long.MaxValue && d > 0 && d <= long.MaxValue) return new((long)n, (long)d, true);
    if (n >= (BigInteger)Int128.MinValue && n <= (BigInteger)Int128.MaxValue
        && d > 0 && d <= (BigInteger)Int128.MaxValue)
      return new(Rational<Int128>.Preformed((Int128)n, (Int128)d));
    return new(Rational<BigInteger>.Preformed(n, d));
  }

  // ── GCD helpers ───────────────────────────────────────────────────────────

  private static long Gcd64(long a, long b)
  {
    while (b != 0) { long t = b; b = a % b; a = t; }
    return a;
  }

  private static Int128 Gcd128(Int128 a, Int128 b)
  {
    while (b != 0) { Int128 t = b; b = a % b; a = t; }
    return a;
  }
}
