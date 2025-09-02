using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace kvad_be.Database;

/// <summary>
/// Single source of truth for all DbContext configuration - used by both runtime and design-time
/// </summary>
public static class DbContextConfiguration
{
    /// <summary>
    /// Configure DbContext options with exactly the same settings for runtime and design-time
    /// </summary>
    public static void ConfigureOptions(DbContextOptionsBuilder<AppDbContext> optionsBuilder, string connectionString)
    {
        // Use the same data source factory that runtime uses
        var dataSource = PostgresDataSourceFactory.Create(connectionString);
        optionsBuilder.UseNpgsql(dataSource);
        
        // Add any other common configuration here
        // optionsBuilder.EnableSensitiveDataLogging(); // For development only
        // optionsBuilder.LogTo(Console.WriteLine); // For development only
    }

    /// <summary>
    /// Get connection string from configuration with validation
    /// </summary>
    public static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found. " +
                "Ensure it's configured in appsettings.json or environment variables.");
        }
        return connectionString;
    }

    /// <summary>
    /// Create a configured AppDbContext instance for design-time use
    /// </summary>
    public static AppDbContext CreateForDesignTime(IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        ConfigureOptions(optionsBuilder, connectionString);
        return new AppDbContext(optionsBuilder.Options);
    }

    /// <summary>
    /// Register AppDbContext and related services for runtime use
    /// </summary>
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);

        // Register the data source as a singleton
        services.AddSingleton<NpgsqlDataSource>(_ => PostgresDataSourceFactory.Create(connectionString));

        // Register the DbContext
        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();
            options.UseNpgsql(dataSource);
            
            // Add any environment-specific configuration
            var environment = serviceProvider.GetService<IWebHostEnvironment>();
            if (environment?.IsDevelopment() == true)
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        return services;
    }
}
