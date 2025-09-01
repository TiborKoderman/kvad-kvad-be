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
            return dsb.Build(); // DO NOT dispose; DI owns it
        });

        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            opt.UseNpgsql(sp.GetRequiredService<NpgsqlDataSource>());
        });

        return services;
    }

}
