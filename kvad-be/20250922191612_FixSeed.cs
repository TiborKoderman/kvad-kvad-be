using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/// <inheritdoc />
public partial class FixSeed : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Devices",
            keyColumn: "Id",
            keyValue: new Guid("504b1696-2ad5-4109-ac28-5158965d6675"),
            columns: new[] { "Description", "Name", "Type", "Virtual" },
            values: new object[] { "This is a physical test device.", "Esp32 Test Device", "Physical", false });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Devices",
            keyColumn: "Id",
            keyValue: new Guid("504b1696-2ad5-4109-ac28-5158965d6675"),
            columns: new[] { "Description", "Name", "Type", "Virtual" },
            values: new object[] { "This is another virtual device for testing purposes.", "Virtual Device 2", "Virtual", true });
    }
}
