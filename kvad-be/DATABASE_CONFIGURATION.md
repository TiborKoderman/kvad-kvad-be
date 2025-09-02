# DbContext Configuration - Single Source of Truth

This project now uses a unified configuration system for Entity Framework DbContext that ensures **design-time and runtime contexts are identical**.

## Architecture

### Core Files

1. **`Database/DbContextConfiguration.cs`** - Main configuration class with all shared logic
2. **`Database/AppDbContextFactory.cs`** - Design-time factory (used by EF migrations)
3. **`Database/AppDbContext.cs`** - The actual DbContext class
4. **`Database/PostgresDataSourceFactory.cs`** - Creates Npgsql data sources
5. **`Database/Converters/PostgresTypeMappings.cs`** - Type mappings (currently empty since using JSONB)

### Key Principles

✅ **Single Source of Truth**: All configuration logic is in `DbContextConfiguration`
✅ **Consistent Configuration**: Design-time and runtime use the exact same settings
✅ **Environment Aware**: Supports different configurations per environment
✅ **Type Safety**: Proper namespacing and compile-time checks

## Usage

### Runtime (Program.cs)
```csharp
builder.Services.AddAppDbContext(builder.Configuration);
```

### Design-time (Migrations)
```bash
dotnet ef migrations add <MigrationName> -o Migrations
```

## Configuration Flow

1. **Runtime**: Program.cs → `DbContextConfiguration.AddAppDbContext()` → registers DbContext with DI
2. **Design-time**: EF tooling → `AppDbContextFactory` → `DbContextConfiguration.CreateForDesignTime()` → creates DbContext

Both paths use the same core method: `DbContextConfiguration.ConfigureOptions()`

## Type Mappings

Currently using **JSONB** for complex types (`Dim7`, `Rational`) as configured in `AppDbContext.ConfigureConventions()`.

## Known Issues & Solutions

### Empty Using Statements
Some model classes don't have explicit namespaces, causing EF to generate `using ;` statements.

**Solution**: Run the fix script after generating migrations:
```bash
./fix-migrations.sh
```

## Migration Workflow

1. Make model changes
2. Generate migration: `dotnet ef migrations add <Name> -o Migrations`
3. Fix empty using statements: `./fix-migrations.sh`
4. Build and test: `dotnet build`
5. Apply migration: `dotnet ef database update`

## Benefits

- ✅ No configuration drift between design-time and runtime
- ✅ Single place to modify database configuration
- ✅ Consistent behavior across environments
- ✅ Easier debugging and maintenance
- ✅ Type-safe configuration
