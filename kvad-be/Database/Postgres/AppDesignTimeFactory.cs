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
    MapAllPgComposites(dsb);
    var ds = dsb.Build();

    var opts = new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(ds)
        .Options;

    return new AppDbContext(opts);
  }

  public static void MapAllPgComposites(NpgsqlDataSourceBuilder dsb)
  {
    var mapComposite = typeof(NpgsqlDataSourceBuilder).GetMethods()
        .First(m => m.Name == "MapComposite" && m.IsGenericMethod && m.GetParameters()[0].ParameterType == typeof(string));

    foreach (var t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.DefinedTypes))
    {
      var attr = t.GetCustomAttribute<PgCompositeAttribute>();
      if (attr is null) continue;

      // Invoke dsb.MapComposite<T>(attr.TypeName) via reflection
      mapComposite.MakeGenericMethod(t.AsType()).Invoke(dsb, new object?[] { attr.TypeName });
    }
  }
}
