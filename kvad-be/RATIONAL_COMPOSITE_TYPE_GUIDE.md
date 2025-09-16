# PostgreSQL Composite Type Implementation Guide for Rational

This guide shows how to implement native PostgreSQL composite type mapping for the `Rational` type in Entity Framework Core with Npgsql.

## Current Status

✅ PostgreSQL composite type created in migration (`rational`)
✅ Npgsql composite type registration (`MapComposite<Rational>("rational")`)
✅ Type mapping classes created
❌ EF Core type mapping plugin integration (needs configuration)

## Complete Implementation Steps

### 1. PostgreSQL Composite Type (✅ Complete)

```sql
CREATE TYPE rational AS (
    Numerator BIGINT,
    Denominator BIGINT
);
```

### 2. Npgsql Registration (✅ Complete)

```csharp
// In PostgresDataSourceFactory.cs
dsb.MapComposite<Rational>("rational");
```

### 3. Insert Data Using Native Syntax

Raw SQL examples for inserting/querying rational data:

```sql
-- Insert using ROW constructor
INSERT INTO "Units" ("Symbol", "Name", "Factor") 
VALUES ('test', 'Test Unit', ROW(3, 4)::rational);

-- Query rational components
SELECT "Symbol", ("Factor").Numerator, ("Factor").Denominator 
FROM "Units" 
WHERE "Symbol" = 'test';

-- Update using ROW constructor
UPDATE "Units" 
SET "Factor" = ROW(5, 6)::rational 
WHERE "Symbol" = 'test';
```

### 4. Working Approaches

#### Approach A: JSONB (Current - Working)
- Uses RationalConverter with JSON serialization
- Stores as `jsonb` in PostgreSQL
- Works reliably with EF Core migrations
- Performance: Good for most use cases

#### Approach B: Native Composite Type (Advanced)
- Uses PostgreSQL's native `rational` type
- Requires complex EF Core type mapping configuration
- Best performance for bulk operations
- More complex to set up

### 5. Example Usage in C#

```csharp
// Create a rational number
var rational = new Rational(3, 4);

// Store in entity
var unit = new LinearUnit
{
    Symbol = "test",
    Name = "Test Unit",
    Factor = rational  // Will be serialized according to configured converter
};

context.Units.Add(unit);
await context.SaveChangesAsync();
```

### 6. Migration Example

```csharp
// In a migration Up() method
migrationBuilder.Sql(@"
    INSERT INTO ""Units"" (""Symbol"", ""Name"", ""Factor"") 
    VALUES ('example', 'Example Unit', ROW(1, 2)::rational);
");
```

## Implementation Notes

1. **Current Setup**: Using JSONB storage with RationalConverter for reliability
2. **Native Composite**: Available via raw SQL in migrations and stored procedures
3. **Performance**: JSONB is fast for most operations; composite types excel in bulk operations
4. **Migrations**: Both approaches work; composite type allows native PostgreSQL operations

## Testing the Implementation

```csharp
// Test rational arithmetic in PostgreSQL
migrationBuilder.Sql(@"
    SELECT 
        ROW(1, 2)::rational AS r1,
        ROW(3, 4)::rational AS r2,
        -- Could implement custom operators for rational arithmetic
        (ROW(1, 2)::rational).Numerator * (ROW(3, 4)::rational).Denominator AS cross_multiply;
");
```

This implementation provides both reliability (JSONB) and native PostgreSQL capabilities (composite type) for optimal flexibility.
