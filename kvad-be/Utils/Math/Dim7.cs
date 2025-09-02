
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
      public sealed class ArrayConverter : ValueConverter<Dim7, short[]>
      {
        public ArrayConverter() : base(
            v => ToArray(v),
            v => FromArray(v))
        { }

        public static short[] ToArray(Dim7 d)
        {
          return [d.L, d.M, d.T, d.I, d.Th, d.N, d.J];
        }

        public static Dim7 FromArray(short[] arr)
        {
          if (arr.Length != 7)
            throw new ArgumentException("Array must be exactly 7 elements long.", nameof(arr));

          return new Dim7(arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]);
        }
      }
}

