// Infrastructure/Postgres/ServiceCollectionExtensions.cs
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddSingleton(_ =>
        {
            var dsb = new NpgsqlDataSourceBuilder(cs);
            PostgresTypeMappings.Apply(dsb);
            MapAllPgComposites(dsb);
            return dsb.Build(); // DO NOT dispose; DI owns it
        });

        services.AddSingleton<IRelationalTypeMappingSourcePlugin, PgCompositeTypeMappingPlugin>();

        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            opt.UseNpgsql(sp.GetRequiredService<NpgsqlDataSource>());
        });

        return services;
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
