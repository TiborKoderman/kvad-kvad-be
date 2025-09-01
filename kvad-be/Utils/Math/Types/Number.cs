using System.Numerics;

public readonly struct Number
{
    private readonly object _value;

    private Number(object value) => _value = value;
    public static implicit operator Number(int v) => new(v);
    public static implicit operator Number(double v) => new(v);
    public static implicit operator Number(BigInteger v) => new(v);
    public static implicit operator Number(Complex v) => new(v);
    public static implicit operator Number(decimal v) => new(v);
    public static implicit operator Number(Rational v) => new(v);

    public T As<T>() => (T)_value;
    public override string ToString() => _value.ToString()!;
}
