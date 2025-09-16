using Microsoft.EntityFrameworkCore.Storage;

public sealed class RationalTypeMappingPlugin : IRelationalTypeMappingSourcePlugin
{
    private static readonly RationalTypeMapping _mapping = new();

    public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        // Only handle Rational type specifically - let other plugins handle other types
        if (mappingInfo.ClrType == typeof(Rational))
            return _mapping;

        if (string.Equals(mappingInfo.StoreTypeNameBase, "rational", StringComparison.OrdinalIgnoreCase))
            return _mapping;

        // Return null for all other types to let the default plugins handle them
        return null;
    }
}
