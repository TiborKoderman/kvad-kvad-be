using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class TestRationalCompositeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Example of inserting data using native PostgreSQL composite type syntax
            migrationBuilder.Sql(@"
                -- Test inserting a rational number using ROW syntax
                INSERT INTO ""Units"" (""Symbol"", ""Name"", ""Quantity"", ""Dimension"", ""Factor"", ""UnitKind"", ""Prefixable"")
                VALUES ('test', 'Test Unit', 'Test', ARRAY[0,0,0,0,0,0,0], ROW(3, 4)::rational, 'linear', true);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""Units"" WHERE ""Symbol"" = 'test';
            ");
        }
    }
}
