using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class FixedTagSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Virtual",
                table: "Tags");

            migrationBuilder.AddColumn<bool>(
                name: "Virtual",
                table: "TagSources",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "TagSources",
                keyColumn: "Id",
                keyValue: 1,
                column: "Virtual",
                value: true);

            migrationBuilder.UpdateData(
                table: "TagSources",
                keyColumn: "Id",
                keyValue: 2,
                column: "Virtual",
                value: true);

            migrationBuilder.UpdateData(
                table: "TagSources",
                keyColumn: "Id",
                keyValue: 3,
                column: "Virtual",
                value: false);

            migrationBuilder.UpdateData(
                table: "TagSources",
                keyColumn: "Id",
                keyValue: 4,
                column: "Virtual",
                value: false);

            migrationBuilder.UpdateData(
                table: "TagSources",
                keyColumn: "Id",
                keyValue: 5,
                column: "Virtual",
                value: false);

            migrationBuilder.UpdateData(
                table: "TagSources",
                keyColumn: "Id",
                keyValue: 6,
                column: "Virtual",
                value: false);

            migrationBuilder.UpdateData(
                table: "TagSources",
                keyColumn: "Id",
                keyValue: 7,
                column: "Virtual",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Virtual",
                table: "TagSources");

            migrationBuilder.AddColumn<bool>(
                name: "Virtual",
                table: "Tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
