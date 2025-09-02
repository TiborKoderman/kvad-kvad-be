using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

public sealed class AppDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    var config = new ConfigurationBuilder()
      .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json", optional: true)
      .AddEnvironmentVariables()
      .Build();

    var cs = config.GetConnectionString("DefaultConnection")
         ?? "Host=localhost;Port=5432;Database=db;Username=postgres;Password=postgres";

    try
    {
        var dsb = new NpgsqlDataSourceBuilder(cs);
        
        // DO NOT apply composite type mappings at design time
        // This prevents issues with entities that haven't been created yet
        // PostgresTypeMappings.Apply(dsb);
        
        var ds = dsb.Build();

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ds)
            .Options;

        return new AppDbContext(opts);
    }
    catch (Exception ex)
    {
        // Fallback for design-time when database is not available
        Console.WriteLine($"Warning: Database connection failed at design time: {ex.Message}");
        
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs, options => {
                // Add some basic configuration that doesn't require connection
            })
            .Options;

        return new AppDbContext(opts);
    }
  }
}
