using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class Dashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "scrollable",
                table: "Dashboards",
                newName: "Scrollable");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "DashboardItems",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<JsonDocument>(
                name: "Config",
                table: "DashboardItems",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Config",
                table: "DashboardItems");

            migrationBuilder.RenameColumn(
                name: "Scrollable",
                table: "Dashboards",
                newName: "scrollable");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "DashboardItems",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
