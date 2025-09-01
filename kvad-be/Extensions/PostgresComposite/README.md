# PostgreSQL Composite Type Mapping for Entity Framework

This library provides automatic mapping of PostgreSQL composite types to C# classes and structs in Entity Framework Core.

## Features

- **Automatic Type Discovery**: Automatically discovers and maps types decorated with `[PgCompositeType]`
- **Flexible Mapping**: Support for both classes and structs
- **Custom Field Mapping**: Map C# properties to specific PostgreSQL composite type fields
- **Automatic Type Creation**: Optionally create/update PostgreSQL composite types during migrations
- **Schema Support**: Support for composite types in different schemas
- **Type Safety**: Full C# type safety with PostgreSQL composite types

## Quick Start

### 1. Decorate Your Types

```csharp
using kvad_be.Extensions.PostgresComposite;

// Example 1: Fully auto-inferred (recommended)
[PgCompositeType("complex_number")]
public readonly struct ComplexNumber
{
    // Auto-inferred: field name = "Real", type = "DOUBLE PRECISION"
    public double Real { get; }
    
    // Auto-inferred: field name = "Imaginary", type = "DOUBLE PRECISION"
    public double Imaginary { get; }

    public ComplexNumber(double real, double imaginary)
    {
        Real = real;
        Imaginary = imaginary;
    }
}

// Example 2: Mixed auto-inference with explicit overrides
[PgCompositeType("dim7")]
public sealed class Dim7
{
    // Auto-inferred: field name = "L", type = "SMALLINT"
    public short L { get; }
    
    // Auto-inferred: field name = "M", type = "SMALLINT"
    public short M { get; }
    
    // Explicit override when needed
    [PgCompositeField("Th", PgType = "SMALLINT")]
    public short Th { get; }
    
    // Auto-inferred: field name = "N", type = "SMALLINT"  
    public short N { get; }
}
```

### 2. Configure Your DbContext

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    // Automatically configure all composite types
    configurationBuilder.MapAllCompositeTypes();
}
```

### 3. Configure Npgsql Data Source

```csharp
services.AddSingleton(_ =>
{
    var dsb = new NpgsqlDataSourceBuilder(connectionString);
    
    // Automatically map all composite types
    dsb.MapAllCompositeTypes();
    
    return dsb.Build();
});
```

### 4. Create Migrations

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Automatically create all composite types
    migrationBuilder.CreateCompositeTypes();
    
    // Your other migration code...
}
```

## Usage Examples

### Basic Struct Example

```csharp
// Fully auto-inferred - no attributes needed on properties!
[PgCompositeType("dim7")]
public readonly struct Dim7
{
    public short L { get; }  // → field "L" SMALLINT
    public short M { get; }  // → field "M" SMALLINT
    public short T { get; }  // → field "T" SMALLINT
    
    // Only override when you need custom names or types
    [PgCompositeField("Th", PgType = "SMALLINT")]
    public short Th { get; }
}

// Auto-inferred result: CREATE TYPE dim7 AS (L SMALLINT, M SMALLINT, T SMALLINT, Th SMALLINT);
```

### Class with Schema

```csharp
[PgCompositeType("point3d", Schema = "geometry")]
public class Point3D
{
    [PgCompositeField("x", PgType = "DOUBLE PRECISION")]
    public double X { get; set; }
    
    [PgCompositeField("y", PgType = "DOUBLE PRECISION")]
    public double Y { get; set; }
    
    [PgCompositeField("z", PgType = "DOUBLE PRECISION")]
    public double Z { get; set; }
}
```

### Using in Entity Classes

```csharp
public class MyEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    // These are automatically mapped to composite types
    public ComplexNumber ComplexValue { get; set; }
    public Point3D Location { get; set; }
    public Dim7 Dimension { get; set; }
}
```

## Attributes

### `[PgCompositeType]`

Marks a class or struct as a PostgreSQL composite type.

**Parameters:**
- `typeName` (required): The name of the PostgreSQL composite type
- `Schema` (optional): The schema containing the type (default: "public")
- `AutoCreateType` (optional): Whether to auto-create the type in migrations (default: true)

### `[PgCompositeField]`

Maps a property to a specific field in the PostgreSQL composite type.

**Parameters:**
- `fieldName` (required): The name of the field in PostgreSQL
- `PgType` (optional): The PostgreSQL data type (auto-inferred if not specified)

## Type Inference

The library automatically infers PostgreSQL types from C# types:

| C# Type | PostgreSQL Type |
|---------|----------------|
| `bool` | `BOOLEAN` |
| `short` | `SMALLINT` |
| `int` | `INTEGER` |
| `long` | `BIGINT` |
| `float` | `REAL` |
| `double` | `DOUBLE PRECISION` |
| `decimal` | `NUMERIC` |
| `string` | `TEXT` |
| `DateTime` | `TIMESTAMP` |
| `DateTimeOffset` | `TIMESTAMPTZ` |
| `TimeSpan` | `INTERVAL` |
| `Guid` | `UUID` |

## Advanced Usage

### Manual Type Mapping

If you prefer manual control:

```csharp
// In ConfigureConventions
configurationBuilder.MapCompositeType<ComplexNumber>();
configurationBuilder.MapCompositeValueType<Dim7>();

// In Npgsql configuration
dsb.MapCompositeType<ComplexNumber>();
```

### Custom Migration Control

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Create specific types
    var complexInfo = PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo(typeof(ComplexNumber));
    migrationBuilder.CreateCompositeType(complexInfo);
    
    // Update existing types
    migrationBuilder.UpdateCompositeTypes();
}
```

### Disable Auto-Creation

```csharp
[PgCompositeType("my_type", AutoCreateType = false)]
public class MyType
{
    // This type won't be auto-created in migrations
}
```

## Migration Management

The library provides several migration helpers:

- `CreateCompositeTypes()`: Creates all composite types
- `UpdateCompositeTypes()`: Updates existing types (drops and recreates)
- `DropCompositeTypes()`: Drops all composite types
- `CreateCompositeType(CompositeTypeInfo)`: Creates a specific type
- `UpdateCompositeType(CompositeTypeInfo)`: Updates a specific type

## Integration with Existing Code

This library is designed to work alongside existing Npgsql composite type mappings. You can gradually migrate existing types to use the new attributes while keeping the old `dsb.MapComposite<T>()` calls for compatibility.

## Best Practices

1. **Use readonly structs** for immutable value types
2. **Specify explicit PostgreSQL types** for better control
3. **Use schemas** to organize related types
4. **Test migrations** in development environments first
5. **Consider versioning** for type schema changes

## Troubleshooting

### Common Issues

1. **Type not found**: Ensure the type is decorated with `[PgCompositeType]`
2. **Migration errors**: Check that `AutoCreateType = true` and the type is properly configured
3. **Field mapping errors**: Verify field names and PostgreSQL types are correct

### Debug Information

The library caches type information for performance. To clear the cache during development:

```csharp
// This is internal, but useful for debugging
// var cache = PgCompositeRelationalTypeMapping._typeInfoCache;
// cache.Clear();
```
