using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using kvad_be.Database;

namespace kvad_be.Extensions.PostgresComposite;

/// <summary>
/// Base migration class that automatically handles composite type creation/updates
/// </summary>
public abstract class CompositeTypeMigration : Migration
{
    protected Assembly? TargetAssembly { get; set; }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create/update composite types before running the main migration
        migrationBuilder.CreateCompositeTypes(TargetAssembly);
        
        // Call the derived migration's Up method
        UpCore(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Call the derived migration's Down method first
        DownCore(migrationBuilder);
        
        // Then drop composite types if needed
        // Note: In practice, you might not want to drop types on rollback
        // migrationBuilder.DropCompositeTypes(TargetAssembly);
    }

    /// <summary>
    /// Override this method instead of Up() to implement your migration logic
    /// </summary>
    protected abstract void UpCore(MigrationBuilder migrationBuilder);

    /// <summary>
    /// Override this method instead of Down() to implement your rollback logic
    /// </summary>
    protected abstract void DownCore(MigrationBuilder migrationBuilder);
}

/// <summary>
/// Migration specifically for creating/updating composite types
/// </summary>
[DbContext(typeof(AppDbContext))]
[Migration("00000000000001_CreateCompositeTypes")]
public partial class CreateCompositeTypes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateCompositeTypes();
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCompositeTypes();
    }
}
