// Infrastructure/Postgres/PostgresDataSourceFactory.cs
using Npgsql;

namespace kvad_be.Database;

public static class PostgresDataSourceFactory
{
    public static NpgsqlDataSource Create(string connectionString)
    {
        var dsb = new NpgsqlDataSourceBuilder(connectionString);

        // Your custom mappings in one place:
        PostgresTypeMappings.Apply(dsb);

        // ADO (Npgsql) dynamic JSON mapping:
        dsb.EnableDynamicJson();

        // (Any other dsb config goes here)
        return dsb.Build();
    }
}
