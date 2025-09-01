using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

public sealed class AppDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
  public AppDbContext CreateDbContext(string[] args)
  {
    var cs = Environment.GetEnvironmentVariable("DefaultConnection")
             ?? "Host=localhost;Port=5432;Database=db;Username=postgres;Password=postgres";

    var dsb = new NpgsqlDataSourceBuilder(cs);
    PostgresTypeMappings.Apply(dsb); // same mappings as runtime
    var ds = dsb.Build();

    var opts = new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(ds)
        .Options;

    return new AppDbContext(opts);
  }


}
