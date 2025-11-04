using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/// <inheritdoc />
public partial class ModifiedDeviceState : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "BatteryPct",
            table: "DeviceState");

        migrationBuilder.DropColumn(
            name: "LastIp",
            table: "DeviceState");

        migrationBuilder.DropColumn(
            name: "LoadPct",
            table: "DeviceState");

        migrationBuilder.DropColumn(
            name: "TempC",
            table: "DeviceState");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:DeviceConnectivity", "online,offline,stale,intermittent,unreachable,unknown")
            .Annotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .Annotation("Npgsql:Enum:DeviceMode", "normal,maintenance,emergency,test,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceConnectivity", "online,offline,intermittent,unreachable,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceMode", "normal,maintenance,emergency,test,unknown");

        migrationBuilder.AddColumn<JsonDocument>(
            name: "AdditionalHealth",
            table: "DeviceState",
            type: "jsonb",
            nullable: true);

        migrationBuilder.UpdateData(
            table: "DeviceState",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            columns: new[] { "AdditionalHealth", "Connectivity" },
            values: new object[] { null, 5 });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AdditionalHealth",
            table: "DeviceState");

        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:DeviceConnectivity", "online,offline,intermittent,unreachable,unknown")
            .Annotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .Annotation("Npgsql:Enum:DeviceMode", "normal,maintenance,emergency,test,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceConnectivity", "online,offline,stale,intermittent,unreachable,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceMode", "normal,maintenance,emergency,test,unknown");

        migrationBuilder.AddColumn<short>(
            name: "BatteryPct",
            table: "DeviceState",
            type: "smallint",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "LastIp",
            table: "DeviceState",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<short>(
            name: "LoadPct",
            table: "DeviceState",
            type: "smallint",
            nullable: true);

        migrationBuilder.AddColumn<float>(
            name: "TempC",
            table: "DeviceState",
            type: "real",
            nullable: true);

        migrationBuilder.UpdateData(
            table: "DeviceState",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            columns: new[] { "BatteryPct", "Connectivity", "LastIp", "LoadPct", "TempC" },
            values: new object[] { null, 4, null, null, null });
    }
}
