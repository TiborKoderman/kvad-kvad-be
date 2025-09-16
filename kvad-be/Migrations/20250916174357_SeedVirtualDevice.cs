using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable
/// <inheritdoc />
public partial class SeedVirtualDevice : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "UptimeSec",
            table: "DeviceState",
            type: "integer",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<long>(
            name: "Seq",
            table: "DeviceState",
            type: "bigint",
            nullable: true,
            oldClrType: typeof(long),
            oldType: "bigint");

        migrationBuilder.InsertData(
            table: "Devices",
            columns: new[] { "Id", "Description", "Location", "Mac", "Name", "OwnerId", "Type", "Virtual" },
            values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), "This is a virtual device for testing purposes.", "Lab", null, "Virtual Device 1", new Guid("cf960f59-cf1f-49cc-8b2c-de4c5e437730"), "Virtual", true });

        migrationBuilder.InsertData(
            table: "DeviceInfo",
            columns: new[] { "DeviceId", "BootId", "Capabilities", "ConfigHash", "Fw", "Hw", "Model", "Settings", "UpdatedAt" },
            values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, null, null, null, null, NodaTime.Instant.FromUnixTimeTicks(17580446370754774L) });

        migrationBuilder.InsertData(
            table: "DeviceState",
            columns: new[] { "DeviceId", "BatteryPct", "BootId", "ConfigHash", "Connectivity", "Extra", "Flags", "HbIntervalSec", "HbJitterPct", "Health", "LastHeartbeat", "LastIp", "Lifecycle", "LoadPct", "Mode", "Rssi", "Seq", "TempC", "UpdatedAt", "UptimeSec" },
            values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), null, null, null, 4, null, null, 10, (short)20, 3, null, null, 0, null, 4, null, 0L, null, NodaTime.Instant.FromUnixTimeTicks(17580446370749758L), 0 });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "DeviceInfo",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

        migrationBuilder.DeleteData(
            table: "DeviceState",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

        migrationBuilder.DeleteData(
            table: "Devices",
            keyColumn: "Id",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

        migrationBuilder.AlterColumn<int>(
            name: "UptimeSec",
            table: "DeviceState",
            type: "integer",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.AlterColumn<long>(
            name: "Seq",
            table: "DeviceState",
            type: "bigint",
            nullable: false,
            defaultValue: 0L,
            oldClrType: typeof(long),
            oldType: "bigint",
            oldNullable: true);
    }
}
