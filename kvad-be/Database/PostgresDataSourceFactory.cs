// Infrastructure/Postgres/PostgresDataSourceFactory.cs
using Npgsql;

namespace kvad_be.Database;

public static class PostgresDataSourceFactory
{
    public static NpgsqlDataSource Create(string connectionString)
    {
        var dsb = new NpgsqlDataSourceBuilder(connectionString);

        // Enable dynamic JSON mapping for JSONB columns
        dsb.EnableDynamicJson();
        dsb.UseNodaTime();

        // Register composite types
        dsb.MapComposite<Rational>("rational");
        // dsb.MapComposite<Dim7>("dim7"); // Uncomment when Dim7 composite mapping is ready
        dsb.MapEnum<DeviceConnectivity>("DeviceConnectivity");
        dsb.MapEnum<DeviceHealth>("DeviceHealth");
        dsb.MapEnum<DeviceMode>("DeviceMode");

        return dsb.Build();
    }
}
