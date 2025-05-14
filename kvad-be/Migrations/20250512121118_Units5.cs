using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class Units5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CronExpression",
                table: "HistoricizationIntervals",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 1,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 2,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 3,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 4,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 5,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 6,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 7,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 8,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 9,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 10,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 11,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 12,
                column: "CronExpression",
                value: null);

            migrationBuilder.UpdateData(
                table: "HistoricizationIntervals",
                keyColumn: "Id",
                keyValue: 13,
                column: "CronExpression",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CronExpression",
                table: "HistoricizationIntervals");
        }
    }
}
