using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;
using kvad_be.Database;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Load configuration (appsettings.json + environment)
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found for design-time.");

        // Build the SAME data source as runtime
        NpgsqlDataSource dataSource = PostgresDataSourceFactory.Create(cs);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(dataSource)
            .Options;

        return new AppDbContext(options);
    }
}