using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace kvad_be.Extensions.PostgresComposite;

/// <summary>
/// Extension methods for configuring PostgreSQL composite types in Entity Framework
/// </summary>
public static class PostgresCompositeExtensions
{
    /// <summary>
    /// Configures a PostgreSQL composite type mapping for Entity Framework
    /// </summary>
    public static ModelConfigurationBuilder MapCompositeType<T>(this ModelConfigurationBuilder builder)
        where T : class
    {
        return MapCompositeType(builder, typeof(T));
    }

    /// <summary>
    /// Configures a PostgreSQL composite type mapping for Entity Framework (for value types/structs)
    /// </summary>
    public static ModelConfigurationBuilder MapCompositeValueType<T>(this ModelConfigurationBuilder builder)
        where T : struct
    {
        return MapCompositeType(builder, typeof(T));
    }

    private static ModelConfigurationBuilder MapCompositeType(ModelConfigurationBuilder builder, Type type)
    {
        var compositeInfo = PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo(type);
        
        builder.Properties(type)
            .HaveColumnType(compositeInfo.FullTypeName);

        return builder;
    }

    /// <summary>
    /// Registers PostgreSQL composite type with Npgsql data source
    /// </summary>
    public static NpgsqlDataSourceBuilder MapCompositeType<T>(this NpgsqlDataSourceBuilder builder)
    {
        var compositeInfo = PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo(typeof(T));
        builder.MapComposite<T>(compositeInfo.TypeName);
        return builder;
    }

    /// <summary>
    /// Auto-discovers and maps all types decorated with [PgCompositeType] attribute
    /// </summary>
    public static NpgsqlDataSourceBuilder MapAllCompositeTypes(this NpgsqlDataSourceBuilder builder, Assembly? assembly = null)
    {
        try
        {
            assembly ??= Assembly.GetCallingAssembly();
            
            var compositeTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<PgCompositeTypeAttribute>() != null);

            foreach (var type in compositeTypes)
            {
                try
                {
                    var compositeInfo = PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo(type);
                    
                    // Create a method info for the generic MapComposite method
                    var mapCompositeMethod = typeof(NpgsqlDataSourceBuilder)
                        .GetMethods()
                        .Where(m => m.Name == "MapComposite" && m.IsGenericMethodDefinition)
                        .FirstOrDefault(m => m.GetParameters().Length == 2);

                    if (mapCompositeMethod != null)
                    {
                        var genericMethod = mapCompositeMethod.MakeGenericMethod(type);
                        genericMethod.Invoke(builder, new object[] { compositeInfo.TypeName });
                    }
                }
                catch (Exception ex)
                {
                    // Log warning but don't fail the entire operation
                    Console.WriteLine($"Warning: Failed to map composite type {type.Name}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            // Log warning but don't fail the entire operation
            Console.WriteLine($"Warning: Failed to discover composite types: {ex.Message}");
        }

        return builder;
    }

    /// <summary>
    /// Auto-discovers and configures all composite types in Entity Framework
    /// </summary>
    public static ModelConfigurationBuilder MapAllCompositeTypes(this ModelConfigurationBuilder builder, Assembly? assembly = null)
    {
        try
        {
            assembly ??= Assembly.GetCallingAssembly();
            
            var compositeTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<PgCompositeTypeAttribute>() != null);

            foreach (var type in compositeTypes)
            {
                try
                {
                    MapCompositeType(builder, type);
                }
                catch (Exception ex)
                {
                    // Log warning but don't fail the entire operation
                    Console.WriteLine($"Warning: Failed to configure composite type {type.Name}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            // Log warning but don't fail the entire operation
            Console.WriteLine($"Warning: Failed to discover composite types for EF configuration: {ex.Message}");
        }

        return builder;
    }

    /// <summary>
    /// Creates a migration to set up composite types
    /// </summary>
    public static void CreateCompositeTypes(this MigrationBuilder migrationBuilder, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        
        var compositeTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<PgCompositeTypeAttribute>() != null)
            .Select(PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo)
            .Where(info => info.AutoCreateType);

        foreach (var compositeInfo in compositeTypes)
        {
            CreateCompositeType(migrationBuilder, compositeInfo);
        }
    }

    /// <summary>
    /// Creates a single composite type
    /// </summary>
    public static void CreateCompositeType(this MigrationBuilder migrationBuilder, CompositeTypeInfo compositeInfo)
    {
        var fields = string.Join(",\n                        ", 
            compositeInfo.Fields.Select(f => $"{f.Name} {f.PgType}"));

        var sql = $@"
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = '{compositeInfo.TypeName}') THEN
                    CREATE TYPE {compositeInfo.FullTypeName} AS (
                        {fields}
                    );
                END IF;
            END
            $$;";

        migrationBuilder.Sql(sql);
    }

    /// <summary>
    /// Drops composite types
    /// </summary>
    public static void DropCompositeTypes(this MigrationBuilder migrationBuilder, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        
        var compositeTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<PgCompositeTypeAttribute>() != null)
            .Select(PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo)
            .Where(info => info.AutoCreateType);

        foreach (var compositeInfo in compositeTypes)
        {
            migrationBuilder.Sql($"DROP TYPE IF EXISTS {compositeInfo.FullTypeName};");
        }
    }

    /// <summary>
    /// Updates composite types to match the current CLR type definitions
    /// </summary>
    public static void UpdateCompositeTypes(this MigrationBuilder migrationBuilder, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        
        var compositeTypes = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<PgCompositeTypeAttribute>() != null)
            .Select(PgCompositeRelationalTypeMapping.GetOrCreateCompositeInfo)
            .Where(info => info.AutoCreateType);

        foreach (var compositeInfo in compositeTypes)
        {
            UpdateCompositeType(migrationBuilder, compositeInfo);
        }
    }

    /// <summary>
    /// Updates a single composite type
    /// </summary>
    public static void UpdateCompositeType(this MigrationBuilder migrationBuilder, CompositeTypeInfo compositeInfo)
    {
        // For safety, we drop and recreate the type
        // In production, you might want more sophisticated ALTER TYPE operations
        migrationBuilder.Sql($"DROP TYPE IF EXISTS {compositeInfo.FullTypeName} CASCADE;");
        CreateCompositeType(migrationBuilder, compositeInfo);
    }
}
