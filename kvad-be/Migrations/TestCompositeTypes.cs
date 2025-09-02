using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using kvad_be.Extensions.PostgresComposite;

namespace kvad_be.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20250902000000_TestCompositeTypes")]
public partial class TestCompositeTypes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // This will automatically create all composite types found in the assembly
        migrationBuilder.CreateCompositeTypes();
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop the composite types on rollback (optional)
        migrationBuilder.DropCompositeTypes();
    }
}
