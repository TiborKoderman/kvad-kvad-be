// Infrastructure/Postgres/ServiceCollectionExtensions.cs
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using kvad_be.Database;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.TryAddEnumerable(
        ServiceDescriptor.Singleton<IRelationalTypeMappingSourcePlugin, PgCompositeMappingPlugin>());

        services.AddSingleton(_ =>
        {
            return PostgresDataSourceFactory.Create(cs);
        });


        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
                        var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
            opt.UseNpgsql(dataSource); // <-- EF-sid
        });

        return services;
    }

}
