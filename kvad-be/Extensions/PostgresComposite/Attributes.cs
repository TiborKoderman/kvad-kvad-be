using System.Reflection;

namespace kvad_be.Extensions.PostgresComposite;

/// <summary>
/// Attribute to mark classes/structs that should be mapped to PostgreSQL composite types
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class PgCompositeTypeAttribute : Attribute
{
    /// <summary>
    /// The name of the PostgreSQL composite type
    /// </summary>
    public string TypeName { get; }
    
    /// <summary>
    /// The schema containing the composite type (defaults to 'public')
    /// </summary>
    public string Schema { get; set; } = "public";
    
    /// <summary>
    /// Whether to automatically create/update the PostgreSQL type during migrations
    /// </summary>
    public bool AutoCreateType { get; set; } = true;

    public PgCompositeTypeAttribute(string typeName)
    {
        TypeName = typeName;
    }
}

/// <summary>
/// Attribute to map property names to PostgreSQL composite type field names
/// If not specified, the property name will be used as the field name and the PostgreSQL type will be auto-inferred
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class PgCompositeFieldAttribute : Attribute
{
    /// <summary>
    /// The name of the field in the PostgreSQL composite type
    /// If not specified, the property/field name will be used
    /// </summary>
    public string? FieldName { get; }
    
    /// <summary>
    /// The PostgreSQL data type for this field (e.g., "SMALLINT", "BIGINT", "TEXT")
    /// If not specified, the type will be auto-inferred from the C# type
    /// </summary>
    public string? PgType { get; set; }

    /// <summary>
    /// Creates a composite field attribute with auto-inferred field name and type
    /// </summary>
    public PgCompositeFieldAttribute()
    {
        FieldName = null;
    }

    /// <summary>
    /// Creates a composite field attribute with explicit field name
    /// </summary>
    /// <param name="fieldName">The PostgreSQL field name</param>
    public PgCompositeFieldAttribute(string fieldName)
    {
        FieldName = fieldName;
    }
}
