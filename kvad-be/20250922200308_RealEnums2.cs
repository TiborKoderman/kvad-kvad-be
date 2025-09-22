using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/// <inheritdoc />
public partial class RealEnums2 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:DeviceConnectivity", "online,offline,intermittent,unreachable,unknown")
            .Annotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .Annotation("Npgsql:Enum:DeviceMode", "normal,maintenance,emergency,test,unknown")
            .OldAnnotation("Npgsql:Enum:device_connectivity", "online,offline,intermittent,unreachable,unknown")
            .OldAnnotation("Npgsql:Enum:device_health", "healthy,warning,critical,unknown")
            .OldAnnotation("Npgsql:Enum:device_mode", "normal,maintenance,emergency,test,unknown");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("Npgsql:Enum:device_connectivity", "online,offline,intermittent,unreachable,unknown")
            .Annotation("Npgsql:Enum:device_health", "healthy,warning,critical,unknown")
            .Annotation("Npgsql:Enum:device_mode", "normal,maintenance,emergency,test,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceConnectivity", "online,offline,intermittent,unreachable,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceHealth", "healthy,warning,critical,unknown")
            .OldAnnotation("Npgsql:Enum:DeviceMode", "normal,maintenance,emergency,test,unknown");
    }
}
