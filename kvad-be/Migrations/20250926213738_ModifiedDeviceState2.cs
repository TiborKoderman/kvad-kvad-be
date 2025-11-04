using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/// <inheritdoc />
public partial class ModifiedDeviceState2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:DeviceConnectivity", "online,offline,stale,intermittent,unreachable,unknown")
            .Annotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .Annotation("Npgsql:Enum:DeviceMode", "normal,low_power,sleep,active,idle,maintenance,emergency,test,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceConnectivity", "online,offline,stale,intermittent,unreachable,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceMode", "normal,maintenance,emergency,test,unknown");

        migrationBuilder.UpdateData(
            table: "DeviceState",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            column: "Mode",
            value: 8);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:DeviceConnectivity", "online,offline,stale,intermittent,unreachable,unknown")
            .Annotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .Annotation("Npgsql:Enum:DeviceMode", "normal,maintenance,emergency,test,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceConnectivity", "online,offline,stale,intermittent,unreachable,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceMode", "normal,low_power,sleep,active,idle,maintenance,emergency,test,unknown");

        migrationBuilder.UpdateData(
            table: "DeviceState",
            keyColumn: "DeviceId",
            keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
            column: "Mode",
            value: 4);
    }
}
