using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DashboardTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    VueComponent = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistorizationIntervals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Interval = table.Column<TimeSpan>(type: "interval", nullable: true),
                    CronExpression = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorizationIntervals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeyValues",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyValues", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    observedServices = table.Column<List<string>>(type: "text[]", nullable: false),
                    observedContainers = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScadaObjectTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<JsonObject>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScadaObjectTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Virtual = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Parameter = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Dimension = table.Column<string>(type: "text", nullable: true),
                    Prefixable = table.Column<bool>(type: "boolean", nullable: false),
                    BaseUnitRelation = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WidgetTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    VueComponent = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    PrivateGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_Username", x => x.Username);
                    table.ForeignKey(
                        name: "FK_Users_Groups_PrivateGroupId",
                        column: x => x.PrivateGroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SIPrefixes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Factor = table.Column<double>(type: "double precision", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SIPrefixes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SIPrefixes_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChatRoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => new { x.ChatRoomId, x.Id });
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatRoomUser",
                columns: table => new
                {
                    ChatRoomsId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomUser", x => new { x.ChatRoomsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ChatRoomUser_ChatRooms_ChatRoomsId",
                        column: x => x.ChatRoomsId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatRoomUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dashboards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TypeId = table.Column<string>(type: "text", nullable: false),
                    DashboardTypeId = table.Column<string>(type: "text", nullable: true),
                    Scrollable = table.Column<bool>(type: "boolean", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dashboards_DashboardTypes_DashboardTypeId",
                        column: x => x.DashboardTypeId,
                        principalTable: "DashboardTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dashboards_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Mac = table.Column<PhysicalAddress>(type: "macaddr", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Virtual = table.Column<bool>(type: "boolean", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupUser",
                columns: table => new
                {
                    GroupsId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUser", x => new { x.GroupsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_GroupUser_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                columns: table => new
                {
                    RolesId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DashboardGroup",
                columns: table => new
                {
                    DashboardsId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardGroup", x => new { x.DashboardsId, x.GroupsId });
                    table.ForeignKey(
                        name: "FK_DashboardGroup_Dashboards_DashboardsId",
                        column: x => x.DashboardsId,
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DashboardGroup_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Widgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeId = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    DashboardId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Widgets_Dashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Widgets_WidgetTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "WidgetTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeviceGroup",
                columns: table => new
                {
                    DevicesId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceGroup", x => new { x.DevicesId, x.GroupsId });
                    table.ForeignKey(
                        name: "FK_DeviceGroup_Devices_DevicesId",
                        column: x => x.DevicesId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceGroup_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceState",
                columns: table => new
                {
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Online = table.Column<bool>(type: "boolean", nullable: false),
                    Connected = table.Column<bool>(type: "boolean", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceState", x => x.DeviceId);
                    table.ForeignKey(
                        name: "FK_DeviceState_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<string>(type: "text", nullable: false),
                    UnitId = table.Column<int>(type: "integer", nullable: true),
                    SourceId = table.Column<int>(type: "integer", nullable: false),
                    Expression = table.Column<string>(type: "text", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    Historize = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => new { x.DeviceId, x.Id });
                    table.ForeignKey(
                        name: "FK_Tags_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tags_TagSources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "TagSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tags_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Layouts",
                columns: table => new
                {
                    DashboardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    Width = table.Column<string>(type: "text", nullable: true),
                    Height = table.Column<string>(type: "text", nullable: true),
                    ParentDashboardId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    WidgetId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layouts", x => new { x.DashboardId, x.Id });
                    table.ForeignKey(
                        name: "FK_Layouts_Dashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Layouts_Layouts_ParentDashboardId_ParentId",
                        columns: x => new { x.ParentDashboardId, x.ParentId },
                        principalTable: "Layouts",
                        principalColumns: new[] { "DashboardId", "Id" });
                    table.ForeignKey(
                        name: "FK_Layouts_Widgets_WidgetId",
                        column: x => x.WidgetId,
                        principalTable: "Widgets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TagHists",
                columns: table => new
                {
                    TagDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<JsonValue>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagHists", x => new { x.TagDeviceId, x.TagId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_TagHists_Tags_TagDeviceId_TagId",
                        columns: x => new { x.TagDeviceId, x.TagId },
                        principalTable: "Tags",
                        principalColumns: new[] { "DeviceId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DashboardTypes",
                columns: new[] { "Id", "Description", "Name", "VueComponent" },
                values: new object[,]
                {
                    { "custom", "Custom dashboard type", "Custom", "CustomDashboard.vue" },
                    { "grid", "Grid dashboard type", "Grid", "GridDashboard.vue" },
                    { "masonry", "Masonry dashboard type", "Masonry", "MasonryDashboard.vue" },
                    { "scada", "Scada dashboard type", "Scada", "ScadaDashboard.vue" }
                });

            migrationBuilder.InsertData(
                table: "HistorizationIntervals",
                columns: new[] { "Id", "CronExpression", "Interval", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "Immediate" },
                    { 2, null, new TimeSpan(0, 0, 0, 1, 0), "1s" },
                    { 3, null, new TimeSpan(0, 0, 0, 10, 0), "10s" },
                    { 4, null, new TimeSpan(0, 0, 1, 0, 0), "1m" },
                    { 5, null, new TimeSpan(0, 0, 5, 0, 0), "5m" },
                    { 6, null, new TimeSpan(0, 0, 10, 0, 0), "10m" },
                    { 7, null, new TimeSpan(0, 0, 15, 0, 0), "15m" },
                    { 8, null, new TimeSpan(0, 0, 30, 0, 0), "30m" },
                    { 9, null, new TimeSpan(0, 1, 0, 0, 0), "1h" },
                    { 10, null, new TimeSpan(0, 6, 0, 0, 0), "6h" },
                    { 11, null, new TimeSpan(0, 12, 0, 0, 0), "12h" },
                    { 12, null, new TimeSpan(1, 0, 0, 0, 0), "Daily" },
                    { 13, null, new TimeSpan(7, 0, 0, 0, 0), "Weekly" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Moderator" },
                    { 3, "User" }
                });

            migrationBuilder.InsertData(
                table: "SIPrefixes",
                columns: new[] { "Id", "Factor", "Name", "Symbol", "UnitId" },
                values: new object[,]
                {
                    { 1, 9.9999999999999998E+23, "Yotta", "Y", null },
                    { 2, 1E+21, "Zetta", "Z", null },
                    { 3, 1E+18, "Exa", "E", null },
                    { 4, 1000000000000000.0, "Peta", "P", null },
                    { 5, 1000000000000.0, "Tera", "T", null },
                    { 6, 1000000000.0, "Giga", "G", null },
                    { 7, 1000000.0, "Mega", "M", null },
                    { 8, 1000.0, "Kilo", "k", null },
                    { 9, 100.0, "Hecto", "h", null },
                    { 10, 10.0, "Deca", "da", null },
                    { 11, 0.10000000000000001, "Deci", "d", null },
                    { 12, 0.01, "Centi", "c", null },
                    { 13, 0.001, "Milli", "m", null },
                    { 14, 9.9999999999999995E-07, "Micro", "μ", null },
                    { 15, 1.0000000000000001E-09, "Nano", "n", null },
                    { 16, 9.9999999999999998E-13, "Pico", "p", null },
                    { 17, 1.0000000000000001E-15, "Femto", "f", null },
                    { 18, 1.0000000000000001E-18, "Atto", "a", null },
                    { 19, 9.9999999999999991E-22, "Zepto", "z", null },
                    { 20, 9.9999999999999992E-25, "Yocto", "y", null }
                });

            migrationBuilder.InsertData(
                table: "TagSources",
                columns: new[] { "Id", "Name", "Virtual" },
                values: new object[,]
                {
                    { 1, "Constant", true },
                    { 2, "Computed", true },
                    { 3, "MQTT", false },
                    { 4, "Modbus", false },
                    { 5, "OPC UA", false },
                    { 6, "HTTP", false },
                    { 7, "WebSocket", false }
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "BaseUnitRelation", "Dimension", "Name", "Parameter", "Prefixable", "Symbol", "Type" },
                values: new object[,]
                {
                    { 1, null, "L", "Meter", "Length", true, "m", 0 },
                    { 2, null, "M", "Kilogram", "Mass", true, "kg", 0 },
                    { 3, null, "T", "Second", "Time", true, "s", 0 },
                    { 4, null, "I", "Ampere", "Electric Current", true, "A", 0 },
                    { 5, null, "Θ", "Kelvin", "Temperature", true, "K", 0 },
                    { 6, null, "N", "Mole", "Amount of Substance", true, "mol", 0 },
                    { 7, null, "J", "Candela", "Luminous Intensity", true, "cd", 0 },
                    { 8, null, "T^-1", "Hertz", "Frequency", true, "Hz", 1 },
                    { 9, null, "M L T^-2", "Newton", "Force", true, "N", 1 },
                    { 10, null, "M L^-1 T^-2", "Pascal", "Pressure", true, "Pa", 1 },
                    { 11, null, "M L^2 T^-2", "Joule", "Energy", true, "J", 1 },
                    { 12, null, "M L^2 T^-3", "Watt", "Power", true, "W", 1 },
                    { 13, null, "I T", "Coulomb", "Electric Charge", true, "C", 1 },
                    { 14, null, "M L^2 T^-3 I^-1", "Volt", "Electric Potential", true, "V", 1 },
                    { 15, null, "M^-1 L^-2 T^4 I^2", "Farad", "Capacitance", true, "F", 1 },
                    { 16, null, "M L^2 T^-3 I^-2", "Ohm", "Resistance", true, "Ω", 1 },
                    { 17, null, "M^-1 L^-2 T^3 I^2", "Siemens", "Conductance", true, "S", 1 },
                    { 18, null, "M L^2 T^-2 I^-1", "Weber", "Magnetic Flux", true, "Wb", 1 },
                    { 19, null, "M L^-1 T^-2 I^-1", "Tesla", "Magnetic Flux Density", true, "T", 1 },
                    { 20, null, "M L^2 T^-2 I^-2", "Henry", "Inductance", true, "H", 1 },
                    { 21, null, "J", "Lumen", "Luminous Flux", true, "lm", 1 },
                    { 22, null, "J L^-2", "Lux", "Illuminance", true, "lx", 1 },
                    { 23, null, "T^-1", "Becquerel", "Radioactivity", true, "Bq", 1 },
                    { 24, null, "L^2 T^-2", "Gray", "Absorbed Dose", true, "Gy", 1 },
                    { 25, null, "L^2 T^-2", "Sievert", "Equivalent Dose", true, "Sv", 1 },
                    { 26, null, "N T^-1", "Katal", "Catalytic Activity", true, "kat", 1 },
                    { 27, null, "M L^2 T^-3", "Apparent Power", "Apparent Power", true, "VA", 1 },
                    { 28, null, "M L^2 T^-3", "Reactive Power", "Reactive Power", true, "var", 1 },
                    { 29, null, "M L^2 T^-3", "Active Power", "Active Power", true, "W", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRoomUser_UsersId",
                table: "ChatRoomUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardGroup_GroupsId",
                table: "DashboardGroup",
                column: "GroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_DashboardTypeId",
                table: "Dashboards",
                column: "DashboardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Dashboards_OwnerId",
                table: "Dashboards",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceGroup_GroupsId",
                table: "DeviceGroup",
                column: "GroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_OwnerId",
                table: "Devices",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_UsersId",
                table: "GroupUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_DashboardId",
                table: "Layouts",
                column: "DashboardId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_ParentDashboardId_ParentId",
                table: "Layouts",
                columns: new[] { "ParentDashboardId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_WidgetId",
                table: "Layouts",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersId",
                table: "RoleUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_SIPrefixes_UnitId",
                table: "SIPrefixes",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TagHists_TagId_Timestamp",
                table: "TagHists",
                columns: new[] { "TagId", "Timestamp" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_SourceId",
                table: "Tags",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UnitId",
                table: "Tags",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PrivateGroupId",
                table: "Users",
                column: "PrivateGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_DashboardId",
                table: "Widgets",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_TypeId",
                table: "Widgets",
                column: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatRoomUser");

            migrationBuilder.DropTable(
                name: "DashboardGroup");

            migrationBuilder.DropTable(
                name: "DeviceGroup");

            migrationBuilder.DropTable(
                name: "DeviceState");

            migrationBuilder.DropTable(
                name: "GroupUser");

            migrationBuilder.DropTable(
                name: "HistorizationIntervals");

            migrationBuilder.DropTable(
                name: "KeyValues");

            migrationBuilder.DropTable(
                name: "Layouts");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropTable(
                name: "ScadaObjectTemplates");

            migrationBuilder.DropTable(
                name: "SIPrefixes");

            migrationBuilder.DropTable(
                name: "TagHists");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.DropTable(
                name: "Widgets");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Dashboards");

            migrationBuilder.DropTable(
                name: "WidgetTypes");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "TagSources");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "DashboardTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
