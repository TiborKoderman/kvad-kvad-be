using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable
/// <inheritdoc />
public partial class FixSeedTimestamps : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "DeviceInfo",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            column: "UpdatedAt",
            value: NodaTime.Instant.FromUnixTimeTicks(17265114000000000L));

        migrationBuilder.UpdateData(
            table: "DeviceState",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            column: "UpdatedAt",
            value: NodaTime.Instant.FromUnixTimeTicks(17265114000000000L));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "DeviceInfo",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            column: "UpdatedAt",
            value: NodaTime.Instant.FromUnixTimeTicks(17580446370754774L));

        migrationBuilder.UpdateData(
            table: "DeviceState",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            column: "UpdatedAt",
            value: NodaTime.Instant.FromUnixTimeTicks(17580446370749758L));
    }
}
