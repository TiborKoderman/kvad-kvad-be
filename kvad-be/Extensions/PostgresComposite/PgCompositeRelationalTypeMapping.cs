using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;
using NpgsqlTypes;

namespace kvad_be.Extensions.PostgresComposite;

/// <summary>
/// Represents a PostgreSQL composite type field
/// </summary>
public record CompositeField(string Name, Type Type, string PgType, PropertyInfo PropertyInfo);

/// <summary>
/// Contains metadata about a PostgreSQL composite type
/// </summary>
public class CompositeTypeInfo
{
    public Type ClrType { get; init; } = null!;
    public string TypeName { get; init; } = null!;
    public string Schema { get; init; } = "public";
    public bool AutoCreateType { get; init; } = true;
    public List<CompositeField> Fields { get; init; } = new();
    public string FullTypeName => $"{Schema}.{TypeName}";
}

/// <summary>
/// Relational type mapping for PostgreSQL composite types that integrates with Entity Framework
/// </summary>
public class PgCompositeRelationalTypeMapping : RelationalTypeMapping
{
    private static readonly ConcurrentDictionary<Type, CompositeTypeInfo> _typeInfoCache = new();
    
    public CompositeTypeInfo CompositeInfo { get; }

    public PgCompositeRelationalTypeMapping(
        Type clrType,
        string storeType,
        CompositeTypeInfo compositeInfo)
        : base(new RelationalTypeMappingParameters(
            new CoreTypeMappingParameters(clrType), 
            storeType))
    {
        CompositeInfo = compositeInfo;
    }

    protected PgCompositeRelationalTypeMapping(RelationalTypeMappingParameters parameters, CompositeTypeInfo compositeInfo)
        : base(parameters)
    {
        CompositeInfo = compositeInfo;
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new PgCompositeRelationalTypeMapping(parameters, CompositeInfo);

    public override DbParameter CreateParameter(
        DbCommand command,
        string name,
        object? value,
        bool? nullable = null,
        ParameterDirection direction = ParameterDirection.Input)
    {
        if (command is NpgsqlCommand npgsqlCommand)
        {
            var npgsqlParameter = npgsqlCommand.CreateParameter();
            npgsqlParameter.Direction = direction;
            npgsqlParameter.ParameterName = name;
            npgsqlParameter.DataTypeName = CompositeInfo.FullTypeName;
            npgsqlParameter.Value = value ?? DBNull.Value;
            return npgsqlParameter;
        }

        var parameter = command.CreateParameter();
        parameter.Direction = direction;
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        return parameter;
    }

    /// <summary>
    /// Gets or creates composite type information for a CLR type
    /// </summary>
    public static CompositeTypeInfo GetOrCreateCompositeInfo(Type clrType)
    {
        return _typeInfoCache.GetOrAdd(clrType, CreateCompositeInfo);
    }

    private static CompositeTypeInfo CreateCompositeInfo(Type clrType)
    {
        var compositeAttr = clrType.GetCustomAttribute<PgCompositeTypeAttribute>()
            ?? throw new InvalidOperationException($"Type {clrType.Name} must be decorated with [PgCompositeType] attribute");

        var fields = new List<CompositeField>();
        var properties = clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite);

        foreach (var prop in properties)
        {
            var fieldAttr = prop.GetCustomAttribute<PgCompositeFieldAttribute>();
            var fieldName = fieldAttr?.FieldName ?? prop.Name; // Use property name if not explicitly set
            var pgType = fieldAttr?.PgType ?? InferPostgresType(prop.PropertyType); // Auto-infer type if not set
            
            fields.Add(new CompositeField(fieldName, prop.PropertyType, pgType, prop));
        }

        // Also check for fields (for structs)
        var structFields = clrType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in structFields)
        {
            // Skip if we already have a property with the same name
            if (fields.Any(f => f.PropertyInfo.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase)))
                continue;

            var fieldAttr = field.GetCustomAttribute<PgCompositeFieldAttribute>();
            var fieldName = fieldAttr?.FieldName ?? field.Name; // Use field name if not explicitly set
            var pgType = fieldAttr?.PgType ?? InferPostgresType(field.FieldType); // Auto-infer type if not set
            
            // Create a fake PropertyInfo for fields (structs)
            var propInfo = new FieldPropertyInfo(field);
            fields.Add(new CompositeField(fieldName, field.FieldType, pgType, propInfo));
        }

        return new CompositeTypeInfo
        {
            ClrType = clrType,
            TypeName = compositeAttr.TypeName,
            Schema = compositeAttr.Schema,
            AutoCreateType = compositeAttr.AutoCreateType,
            Fields = fields
        };
    }

    /// <summary>
    /// Infers PostgreSQL type from CLR type
    /// </summary>
    private static string InferPostgresType(Type clrType)
    {
        var underlyingType = Nullable.GetUnderlyingType(clrType) ?? clrType;

        return underlyingType.Name switch
        {
            nameof(Boolean) => "BOOLEAN",
            nameof(SByte) => "SMALLINT",
            nameof(Byte) => "SMALLINT",
            nameof(Int16) => "SMALLINT",
            nameof(UInt16) => "INTEGER",
            nameof(Int32) => "INTEGER",
            nameof(UInt32) => "BIGINT",
            nameof(Int64) => "BIGINT",
            nameof(UInt64) => "NUMERIC(20,0)",
            nameof(Single) => "REAL",
            nameof(Double) => "DOUBLE PRECISION",
            nameof(Decimal) => "NUMERIC",
            nameof(String) => "TEXT",
            nameof(DateTime) => "TIMESTAMP",
            nameof(DateTimeOffset) => "TIMESTAMPTZ",
            nameof(TimeSpan) => "INTERVAL",
            nameof(Guid) => "UUID",
            _ => underlyingType.IsEnum ? "INTEGER" : "TEXT"
        };
    }
}

/// <summary>
/// Adapter to treat FieldInfo as PropertyInfo for struct fields
/// </summary>
internal class FieldPropertyInfo : PropertyInfo
{
    private readonly FieldInfo _fieldInfo;

    public FieldPropertyInfo(FieldInfo fieldInfo)
    {
        _fieldInfo = fieldInfo;
    }

    public override string Name => _fieldInfo.Name;
    public override Type PropertyType => _fieldInfo.FieldType;
    public override Type? DeclaringType => _fieldInfo.DeclaringType;
    public override Type? ReflectedType => _fieldInfo.ReflectedType;
    public override bool CanRead => true;
    public override bool CanWrite => !_fieldInfo.IsInitOnly;

    public override object? GetValue(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? index, System.Globalization.CultureInfo? culture)
        => _fieldInfo.GetValue(obj);

    public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, object?[]? index, System.Globalization.CultureInfo? culture)
        => _fieldInfo.SetValue(obj, value);

    public override MethodInfo[] GetAccessors(bool nonPublic) => Array.Empty<MethodInfo>();
    public override MethodInfo? GetGetMethod(bool nonPublic) => null;
    public override MethodInfo? GetSetMethod(bool nonPublic) => null;
    public override ParameterInfo[] GetIndexParameters() => Array.Empty<ParameterInfo>();
    public override PropertyAttributes Attributes => PropertyAttributes.None;
    public override object[] GetCustomAttributes(bool inherit) => _fieldInfo.GetCustomAttributes(inherit);
    public override object[] GetCustomAttributes(Type attributeType, bool inherit) => _fieldInfo.GetCustomAttributes(attributeType, inherit);
    public override bool IsDefined(Type attributeType, bool inherit) => _fieldInfo.IsDefined(attributeType, inherit);
}
