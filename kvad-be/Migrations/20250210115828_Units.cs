using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace kvad_be.Migrations
{
    /// <inheritdoc />
    public partial class Units : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Parameter = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Dimension = table.Column<string>(type: "TEXT", nullable: true),
                    Prefixable = table.Column<bool>(type: "INTEGER", nullable: false),
                    BaseUnitRelation = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SIPrefixes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Factor = table.Column<double>(type: "REAL", nullable: false),
                    UnitId = table.Column<int>(type: "INTEGER", nullable: true)
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
                name: "IX_SIPrefixes_UnitId",
                table: "SIPrefixes",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SIPrefixes");

            migrationBuilder.DropTable(
                name: "Units");
        }
    }
}
