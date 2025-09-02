using Microsoft.EntityFrameworkCore.Storage;

sealed class PgCompositeMappingPlugin : IRelationalTypeMappingSourcePlugin
{
    private readonly RelationalTypeMapping _dim7     = new Dim7TypeMapping();
    private readonly RelationalTypeMapping _rational = new RationalTypeMapping();

    public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo info)
    {
        var clr = info.ClrType;
        if (clr == typeof(Dim7))     return _dim7;
        if (clr == typeof(Rational)) return _rational;

        // Optional: honor HasColumnType("dim7"/"rational")
        var store = info.StoreTypeNameBase?.ToLowerInvariant();
        if (store == "dim7")     return _dim7;
        if (store == "rational") return _rational;

        return null;
    }
}
