using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace kvad_be.Extensions.PostgresComposite;

/// <summary>
/// Type mapping source plugin for PostgreSQL composite types
/// </summary>
public class PostgresCompositeTypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
{
    private readonly Assembly _assembly;

    public PostgresCompositeTypeMappingSourcePlugin(Assembly? assembly = null)
    {
        _assembly = assembly ?? Assembly.GetCallingAssembly();
    }

    public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
    {
        var clrType = mappingInfo.ClrType;
        if (clrType == null)
            return null;

        // Check if the type has the PgCompositeType attribute
        var compositeAttr = clrType.GetCustomAttribute<PgCompositeTypeAttribute>();
        if (compositeAttr == null)
            return null;

        var compositeInfo = PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo(clrType);
        var storeType = mappingInfo.StoreTypeName ?? compositeInfo.FullTypeName;

        return new PgCompositeRelationalTypeMapping(clrType, storeType, compositeInfo);
    }
}
