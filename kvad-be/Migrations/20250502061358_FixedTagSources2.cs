using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class FixedTagSources2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricizationIntervals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Interval = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricizationIntervals", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "HistoricizationIntervals",
                columns: new[] { "Id", "Interval", "Name" },
                values: new object[,]
                {
                    { 1, null, "Immediate" },
                    { 2, new TimeSpan(0, 0, 0, 1, 0), "1s" },
                    { 3, new TimeSpan(0, 0, 0, 10, 0), "10s" },
                    { 4, new TimeSpan(0, 0, 1, 0, 0), "1m" },
                    { 5, new TimeSpan(0, 0, 5, 0, 0), "5m" },
                    { 6, new TimeSpan(0, 0, 10, 0, 0), "10m" },
                    { 7, new TimeSpan(0, 0, 15, 0, 0), "15m" },
                    { 8, new TimeSpan(0, 0, 30, 0, 0), "30m" },
                    { 9, new TimeSpan(0, 1, 0, 0, 0), "1h" },
                    { 10, new TimeSpan(0, 6, 0, 0, 0), "6h" },
                    { 11, new TimeSpan(0, 12, 0, 0, 0), "12h" },
                    { 12, new TimeSpan(1, 0, 0, 0, 0), "Daily" },
                    { 13, new TimeSpan(7, 0, 0, 0, 0), "Weekly" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricizationIntervals");
        }
    }
}
