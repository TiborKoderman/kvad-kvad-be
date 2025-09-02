using Microsoft.EntityFrameworkCore.Storage;

public sealed class RationalTypeMappingPlugin : IRelationalTypeMappingSourcePlugin
{
    private static readonly RationalTypeMapping _mapping = new();

    public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        // Match by CLR type or store type name.
        if (mappingInfo.ClrType == typeof(Rational))
            return _mapping;

        if (string.Equals(mappingInfo.StoreTypeNameBase, "rational", StringComparison.OrdinalIgnoreCase))
            return _mapping;

        return null;
    }
}
