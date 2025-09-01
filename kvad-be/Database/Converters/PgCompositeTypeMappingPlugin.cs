using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage;

sealed class PgCompositeTypeMappingPlugin : IRelationalTypeMappingSourcePlugin
{
    // Cache discovered composites: storeType -> CLR type
    static readonly Dictionary<string, Type> _byStore =
        AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.DefinedTypes)
        .Where(t => t.GetCustomAttribute<PgCompositeAttribute>() is not null)
        .ToDictionary(
            t => t.GetCustomAttribute<PgCompositeAttribute>()!.TypeName,
            t => (Type)t);

    public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo info)
    {
        // If the property CLR type is marked, return a mapping
        if (info.ClrType is { } clr &&
            clr.GetCustomAttribute<PgCompositeAttribute>() is { } compAttr)
            return new PgCompositeRelationalTypeMapping(clr, compAttr.TypeName);

        // Or if user wrote .HasColumnType("public.dim7"), map by store type name
        var store = info.StoreTypeNameBase ?? info.StoreTypeName;
        if (store is not null && _byStore.TryGetValue(store, out var mappedClr))
            return new PgCompositeRelationalTypeMapping(mappedClr, store);

        return null;
    }
}
