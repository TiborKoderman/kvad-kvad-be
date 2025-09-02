
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public sealed class Dim7
{
    // Auto-inferred: field name = "L", type = "SMALLINT"
    public short L { get; }

    // Auto-inferred: field name = "M", type = "SMALLINT"
    public short M { get; }

    // Auto-inferred: field name = "T", type = "SMALLINT"
    public short T { get; }

    // Auto-inferred: field name = "I", type = "SMALLINT"
    public short I { get; }

    // Explicit override example - field name "Th", type "SMALLINT"
    public short Th { get; }

    // Auto-inferred: field name = "N", type = "SMALLINT"
    public short N { get; }

    // Auto-inferred: field name = "J", type = "SMALLINT"
    public short J { get; }

    public Dim7(short L, short M, short T, short I, short Th, short N, short J)
    {
        this.L = L;
        this.M = M;
        this.T = T;
        this.I = I;
        this.Th = Th;
        this.N = N;
        this.J = J;
    }


    public static readonly Dim7 Zero = new(0, 0, 0, 0, 0, 0, 0);

    public static Dim7 operator +(Dim7 a, Dim7 b)
        => new((short)(a.L + b.L), (short)(a.M + b.M), (short)(a.T + b.T), (short)(a.I + b.I),
               (short)(a.Th + b.Th), (short)(a.N + b.N), (short)(a.J + b.J));

    public static Dim7 operator -(Dim7 a, Dim7 b)
        => new((short)(a.L - b.L), (short)(a.M - b.M), (short)(a.T - b.T), (short)(a.I - b.I),
               (short)(a.Th - b.Th), (short)(a.N - b.N), (short)(a.J - b.J));

    public static Dim7 operator *(Dim7 a, int k)
        => new((short)(a.L * k), (short)(a.M * k), (short)(a.T * k), (short)(a.I * k),
               (short)(a.Th * k), (short)(a.N * k), (short)(a.J * k));

    public override string ToString()
        => $"[{L},{M},{T},{I},{Th},{N},{J}]";

    public sealed class BytesConverter : ValueConverter<Dim7, byte[]>
    {
        public BytesConverter() : base(
            v => ToBytes(v),
            v => FromBytes(v))
        { }

        public static byte[] ToBytes(Dim7 d)
        {
            Span<byte> bytes = stackalloc byte[14];
            BitConverter.TryWriteBytes(bytes.Slice(0, 2), d.L);
            BitConverter.TryWriteBytes(bytes.Slice(2, 2), d.M);
            BitConverter.TryWriteBytes(bytes.Slice(4, 2), d.T);
            BitConverter.TryWriteBytes(bytes.Slice(6, 2), d.I);
            BitConverter.TryWriteBytes(bytes.Slice(8, 2), d.Th);
            BitConverter.TryWriteBytes(bytes.Slice(10, 2), d.N);
            BitConverter.TryWriteBytes(bytes.Slice(12, 2), d.J);
            return bytes.ToArray();
        }

        public static Dim7 FromBytes(byte[] bytes)
        {
            if (bytes.Length != 14)
                throw new ArgumentException("Byte array must be exactly 14 bytes long.", nameof(bytes));

            return new Dim7(
                BitConverter.ToInt16(bytes, 0),
                BitConverter.ToInt16(bytes, 2),
                BitConverter.ToInt16(bytes, 4),
                BitConverter.ToInt16(bytes, 6),
                BitConverter.ToInt16(bytes, 8),
                BitConverter.ToInt16(bytes, 10),
                BitConverter.ToInt16(bytes, 12)
            );
        }


    }
}

