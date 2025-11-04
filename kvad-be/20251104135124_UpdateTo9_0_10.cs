using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/// <inheritdoc />
public partial class UpdateTo9_0_10 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Devices",
            keyColumn: "Id",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            column: "Lifecycle",
            value: 11);

        migrationBuilder.UpdateData(
            table: "Devices",
            keyColumn: "Id",
            keyValue: new Guid("504b1696-2ad5-4109-ac28-5158965d6675"),
            column: "Lifecycle",
            value: 11);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Devices",
            keyColumn: "Id",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            column: "Lifecycle",
            value: 0);

        migrationBuilder.UpdateData(
            table: "Devices",
            keyColumn: "Id",
            keyValue: new Guid("504b1696-2ad5-4109-ac28-5158965d6675"),
            column: "Lifecycle",
            value: 0);
    }
}
