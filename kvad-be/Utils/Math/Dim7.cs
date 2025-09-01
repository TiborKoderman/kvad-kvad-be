
public readonly record struct Dim7(short L, short M, short T, short I, short Th, short N, short J)
{
    public static readonly Dim7 Zero = new(0,0,0,0,0,0,0);

    public static Dim7 operator +(Dim7 a, Dim7 b)
        => new((short)(a.L+b.L),(short)(a.M+b.M),(short)(a.T+b.T),(short)(a.I+b.I),
               (short)(a.Th+b.Th),(short)(a.N+b.N),(short)(a.J+b.J));

    public static Dim7 operator -(Dim7 a, Dim7 b)
        => new((short)(a.L-b.L),(short)(a.M-b.M),(short)(a.T-b.T),(short)(a.I-b.I),
               (short)(a.Th-b.Th),(short)(a.N-b.N),(short)(a.J-b.J));

    public static Dim7 operator *(Dim7 a, int k)
        => new((short)(a.L*k),(short)(a.M*k),(short)(a.T*k),(short)(a.I*k),
               (short)(a.Th*k),(short)(a.N*k),(short)(a.J*k));

    public override string ToString()
        => $"[{L},{M},{T},{I},{Th},{N},{J}]";
}
