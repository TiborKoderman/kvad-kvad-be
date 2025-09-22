using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/// <inheritdoc />
public partial class SeedRealDevice : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "Devices",
            columns: new[] { "Id", "Description", "Lifecycle", "Location", "Mac", "Name", "OwnerId", "Type", "Virtual" },
            values: new object[] { new Guid("504b1696-2ad5-4109-ac28-5158965d6675"), "This is another virtual device for testing purposes.", 0, "Office", null, "Virtual Device 2", new Guid("cf960f59-cf1f-49cc-8b2c-de4c5e437730"), "Virtual", true });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "Devices",
            keyColumn: "Id",
            keyValue: new Guid("504b1696-2ad5-4109-ac28-5158965d6675"));
    }
}
