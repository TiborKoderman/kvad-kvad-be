using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
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
                name: "Prefixes",
                columns: table => new
                {
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Base = table.Column<short>(type: "smallint", nullable: false),
                    Exponent = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prefixes", x => x.Symbol);
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
                    Data = table.Column<JsonDocument>(type: "jsonb", nullable: false)
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
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<string>(type: "text", nullable: false),
                    Prefixable = table.Column<bool>(type: "boolean", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: true),
                    UnitKind = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Offset = table.Column<decimal>(type: "numeric", nullable: true),
                    LogBase = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Symbol);
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
                name: "UnitCanonicalPart",
                columns: table => new
                {
                    UnitSymbol = table.Column<string>(type: "text", nullable: false),
                    PartSymbol = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitCanonicalPart", x => new { x.UnitSymbol, x.PartSymbol });
                    table.ForeignKey(
                        name: "FK_UnitCanonicalPart_Units_PartSymbol",
                        column: x => x.PartSymbol,
                        principalTable: "Units",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitCanonicalPart_Units_UnitSymbol",
                        column: x => x.UnitSymbol,
                        principalTable: "Units",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Cascade);
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
                    UnitSymbol = table.Column<string>(type: "text", nullable: true),
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
                        name: "FK_Tags_Units_UnitSymbol",
                        column: x => x.UnitSymbol,
                        principalTable: "Units",
                        principalColumn: "Symbol");
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
                name: "TagCurr",
                columns: table => new
                {
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Value = table.Column<JsonValue>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagCurr", x => new { x.DeviceId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagCurr_Tags_DeviceId_TagId",
                        columns: x => new { x.DeviceId, x.TagId },
                        principalTable: "Tags",
                        principalColumns: new[] { "DeviceId", "Id" },
                        onDelete: ReferentialAction.Cascade);
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
                table: "Prefixes",
                columns: new[] { "Symbol", "Base", "Exponent", "Name" },
                values: new object[,]
                {
                    { "a", (short)10, (short)-18, "Atto" },
                    { "c", (short)10, (short)-2, "Centi" },
                    { "d", (short)10, (short)-1, "Deci" },
                    { "da", (short)10, (short)1, "Deca" },
                    { "E", (short)10, (short)18, "Exa" },
                    { "Ei", (short)2, (short)60, "Exbi" },
                    { "f", (short)10, (short)-15, "Femto" },
                    { "G", (short)10, (short)9, "Giga" },
                    { "Gi", (short)2, (short)30, "Gibi" },
                    { "h", (short)10, (short)2, "Hecto" },
                    { "k", (short)10, (short)3, "Kilo" },
                    { "Ki", (short)2, (short)10, "Kibi" },
                    { "m", (short)10, (short)-3, "Milli" },
                    { "M", (short)10, (short)6, "Mega" },
                    { "Mi", (short)2, (short)20, "Mebi" },
                    { "n", (short)10, (short)-9, "Nano" },
                    { "p", (short)10, (short)-12, "Pico" },
                    { "P", (short)10, (short)15, "Peta" },
                    { "Pi", (short)2, (short)50, "Pebi" },
                    { "q", (short)10, (short)-30, "Quecto" },
                    { "Q", (short)10, (short)30, "Quetta" },
                    { "Qi", (short)2, (short)100, "Quin" },
                    { "r", (short)10, (short)-27, "Ronto" },
                    { "R", (short)10, (short)27, "Ronna" },
                    { "Ri", (short)2, (short)90, "Roni" },
                    { "T", (short)10, (short)12, "Tera" },
                    { "Ti", (short)2, (short)40, "Tebi" },
                    { "y", (short)10, (short)-24, "Yocto" },
                    { "Y", (short)10, (short)24, "Yotta" },
                    { "Yi", (short)2, (short)80, "Yobi" },
                    { "z", (short)10, (short)-21, "Zepto" },
                    { "Z", (short)10, (short)21, "Zetta" },
                    { "Zi", (short)2, (short)70, "Zebi" },
                    { "μ", (short)10, (short)-6, "Micro" }
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
                columns: new[] { "Symbol", "Definition", "Name", "Prefixable", "Quantity", "UnitKind" },
                values: new object[,]
                {
                    { "A", null, "Ampere", true, "Electric Current", "linear" },
                    { "cd", null, "Candela", true, "Luminous Intensity", "linear" },
                    { "K", null, "Kelvin", true, "Temperature", "linear" },
                    { "kg", null, "Kilogram", true, "Mass", "linear" },
                    { "m", null, "Meter", true, "Length", "linear" },
                    { "mol", null, "Mole", true, "Amount of Substance", "linear" },
                    { "s", null, "Second", true, "Time", "linear" }
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
                name: "IX_TagHists_TagId_Timestamp",
                table: "TagHists",
                columns: new[] { "TagId", "Timestamp" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_SourceId",
                table: "Tags",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UnitSymbol",
                table: "Tags",
                column: "UnitSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_UnitCanonicalPart_PartSymbol",
                table: "UnitCanonicalPart",
                column: "PartSymbol");

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
                name: "Prefixes");

            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropTable(
                name: "ScadaObjectTemplates");

            migrationBuilder.DropTable(
                name: "TagCurr");

            migrationBuilder.DropTable(
                name: "TagHists");

            migrationBuilder.DropTable(
                name: "UnitCanonicalPart");

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
