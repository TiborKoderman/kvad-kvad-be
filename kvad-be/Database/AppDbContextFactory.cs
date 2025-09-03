using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage;

namespace kvad_be.Database;

/// <summary>
/// Design-time factory for creating AppDbContext instances during migrations and other EF tooling operations.
/// Uses the same configuration as runtime via DbContextConfiguration.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Build configuration exactly like the runtime does
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args) // Support command line overrides
            .Build();

    
        var services = new ServiceCollection().AddSingleton<IRelationalTypeMappingSourcePlugin, RationalTypeMappingPlugin>();
        // Use the shared configuration - single source of truth
        return DbContextConfiguration.CreateForDesignTime(configuration);
    }
}