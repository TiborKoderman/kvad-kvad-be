using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/// <inheritdoc />
public partial class UpdateEnumUnitDimensionConstraints : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // This migration was originally intended to drop a check constraint,
        // but the constraint was removed from the Initial migration to avoid PostgreSQL subquery limitations.
        // No action needed.
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddCheckConstraint(
            name: "ck_enumunitdimension_sequential_ids",
            table: "EnumUnitDimensions",
            sql: "(\"Id\" = 0) OR EXISTS (SELECT 1 FROM \"EnumUnitDimensions\" e WHERE e.\"Id\" = \"EnumUnitDimensions\".\"Id\" - 1)");
    }
}
