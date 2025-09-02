using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;
using kvad_be.Database;

namespace kvad_be.Migrations;

public sealed class AppDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    var config = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: true)
      .AddJsonFile("appsettings.Development.json", optional: true)
      .AddEnvironmentVariables()
      .Build();

    var cs = config.GetConnectionString("DefaultConnection")
         ?? "Host=localhost;Port=5432;Database=db;Username=postgres;Password=postgres";

    try
    {
        var dsb = new NpgsqlDataSourceBuilder(cs);
        
        // Apply composite type mappings for production
        try
        {
            PostgresTypeMappings.Apply(dsb);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not apply composite type mappings: {ex.Message}");
        }
        
        var ds = dsb.Build();

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ds, options => {
                options.MigrationsAssembly("kvad-be");
                options.MigrationsHistoryTable("__EFMigrationsHistory");
            })
            .Options;

        return new AppDbContext(opts);
    }
    catch (Exception ex)
    {
        // Fallback for design-time when database is not available
        Console.WriteLine($"Warning: Database connection failed at design time: {ex.Message}");
        
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs, options => {
                options.MigrationsAssembly("kvad-be");
                options.MigrationsHistoryTable("__EFMigrationsHistory");
            })
            .Options;

        return new AppDbContext(opts);
    }
  }
}
