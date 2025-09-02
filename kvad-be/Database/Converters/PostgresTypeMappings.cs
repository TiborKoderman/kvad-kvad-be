// Infrastructure/Postgres/PostgresTypeMappings.cs
using Npgsql;

namespace kvad_be.Database;

public static class PostgresTypeMappings
{
    public static void Apply(NpgsqlDataSourceBuilder dsb)
    {
        // Since we're using JSONB for Dim7 and Rational in EF Core configuration,
        // we don't need composite type mappings here.
        // The JSONB mapping is handled by EF Core's ConfigureConventions in AppDbContext.
        
        // If you want to use PostgreSQL composite types instead of JSONB, 
        // you would uncomment these lines and remove the JSONB configuration in AppDbContext:
        // dsb.MapComposite<Dim7>("dim7");
        // dsb.MapComposite<Rational>("rational");
        
        // For now, keeping this empty since we use JSONB mapping
    }
}