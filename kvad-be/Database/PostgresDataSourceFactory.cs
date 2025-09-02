// Infrastructure/Postgres/PostgresDataSourceFactory.cs
using Npgsql;

namespace kvad_be.Database;

public static class PostgresDataSourceFactory
{
    public static NpgsqlDataSource Create(string connectionString)
    {
        var dsb = new NpgsqlDataSourceBuilder(connectionString);

        // Map composite types to match the PostgreSQL schema
        dsb.MapComposite<Rational>("rational");
        dsb.MapComposite<Dim7>("dim7");
        
        // ADO (Npgsql) dynamic JSON mapping:
        dsb.EnableDynamicJson();

        // (Any other dsb config goes here)
        return dsb.Build();
    }
}
