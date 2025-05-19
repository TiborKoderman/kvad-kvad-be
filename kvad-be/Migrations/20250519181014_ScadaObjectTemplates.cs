using System;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class ScadaObjectTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricizationIntervals");

            migrationBuilder.RenameColumn(
                name: "Historicize",
                table: "Tags",
                newName: "Historize");

            migrationBuilder.CreateTable(
                name: "HistorizationIntervals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Interval = table.Column<TimeSpan>(type: "interval", nullable: true),
                    CronExpression = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorizationIntervals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScadaObjectTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<JsonObject>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScadaObjectTemplates", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "HistorizationIntervals",
                columns: new[] { "Id", "CronExpression", "Interval", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "Immediate" },
                    { 2, null, new TimeSpan(0, 0, 0, 1, 0), "1s" },
                    { 3, null, new TimeSpan(0, 0, 0, 10, 0), "10s" },
                    { 4, null, new TimeSpan(0, 0, 1, 0, 0), "1m" },
                    { 5, null, new TimeSpan(0, 0, 5, 0, 0), "5m" },
                    { 6, null, new TimeSpan(0, 0, 10, 0, 0), "10m" },
                    { 7, null, new TimeSpan(0, 0, 15, 0, 0), "15m" },
                    { 8, null, new TimeSpan(0, 0, 30, 0, 0), "30m" },
                    { 9, null, new TimeSpan(0, 1, 0, 0, 0), "1h" },
                    { 10, null, new TimeSpan(0, 6, 0, 0, 0), "6h" },
                    { 11, null, new TimeSpan(0, 12, 0, 0, 0), "12h" },
                    { 12, null, new TimeSpan(1, 0, 0, 0, 0), "Daily" },
                    { 13, null, new TimeSpan(7, 0, 0, 0, 0), "Weekly" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorizationIntervals");

            migrationBuilder.DropTable(
                name: "ScadaObjectTemplates");

            migrationBuilder.RenameColumn(
                name: "Historize",
                table: "Tags",
                newName: "Historicize");

            migrationBuilder.CreateTable(
                name: "HistoricizationIntervals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CronExpression = table.Column<string>(type: "text", nullable: true),
                    Interval = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricizationIntervals", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "HistoricizationIntervals",
                columns: new[] { "Id", "CronExpression", "Interval", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "Immediate" },
                    { 2, null, new TimeSpan(0, 0, 0, 1, 0), "1s" },
                    { 3, null, new TimeSpan(0, 0, 0, 10, 0), "10s" },
                    { 4, null, new TimeSpan(0, 0, 1, 0, 0), "1m" },
                    { 5, null, new TimeSpan(0, 0, 5, 0, 0), "5m" },
                    { 6, null, new TimeSpan(0, 0, 10, 0, 0), "10m" },
                    { 7, null, new TimeSpan(0, 0, 15, 0, 0), "15m" },
                    { 8, null, new TimeSpan(0, 0, 30, 0, 0), "30m" },
                    { 9, null, new TimeSpan(0, 1, 0, 0, 0), "1h" },
                    { 10, null, new TimeSpan(0, 6, 0, 0, 0), "6h" },
                    { 11, null, new TimeSpan(0, 12, 0, 0, 0), "12h" },
                    { 12, null, new TimeSpan(1, 0, 0, 0, 0), "Daily" },
                    { 13, null, new TimeSpan(7, 0, 0, 0, 0), "Weekly" }
                });
        }
    }
}
