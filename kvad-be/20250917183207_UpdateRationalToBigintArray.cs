using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
/// <inheritdoc />
public partial class UpdateRationalToBigintArray : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Convert existing JSONB Rational data to bigint[] format
        
        // Convert LogRef column
        migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" 
            ALTER COLUMN ""LogRef"" TYPE bigint[] 
            USING CASE 
                WHEN ""LogRef"" IS NULL THEN NULL
                ELSE ARRAY[
                    (""LogRef""->>'Numerator')::bigint,
                    (""LogRef""->>'Denominator')::bigint
                ]
            END;
        ");

        // Convert LogK column
        migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" 
            ALTER COLUMN ""LogK"" TYPE bigint[] 
            USING CASE 
                WHEN ""LogK"" IS NULL THEN NULL
                ELSE ARRAY[
                    (""LogK""->>'Numerator')::bigint,
                    (""LogK""->>'Denominator')::bigint
                ]
            END;
        ");

        // Convert Factor column
        migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" 
            ALTER COLUMN ""Factor"" TYPE bigint[] 
            USING ARRAY[
                (""Factor""->>'Numerator')::bigint,
                (""Factor""->>'Denominator')::bigint
            ];
        ");

        // Convert Exponent column in UnitCanonicalPart
        migrationBuilder.Sql(@"
            ALTER TABLE ""UnitCanonicalPart"" 
            ALTER COLUMN ""Exponent"" TYPE bigint[] 
            USING ARRAY[
                (""Exponent""->>'Numerator')::bigint,
                (""Exponent""->>'Denominator')::bigint
            ];
        ");

        // Update seed data is now handled automatically by the converter
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "LogRef",
            table: "Units",
            type: "jsonb",
            nullable: true,
            oldClrType: typeof(long[]),
            oldType: "bigint[]",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "LogK",
            table: "Units",
            type: "jsonb",
            nullable: true,
            oldClrType: typeof(long[]),
            oldType: "bigint[]",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Factor",
            table: "Units",
            type: "jsonb",
            nullable: false,
            oldClrType: typeof(long[]),
            oldType: "bigint[]");

        migrationBuilder.AlterColumn<string>(
            name: "Exponent",
            table: "UnitCanonicalPart",
            type: "jsonb",
            nullable: false,
            oldClrType: typeof(long[]),
            oldType: "bigint[]");

        migrationBuilder.UpdateData(
            table: "Units",
            keyColumn: "Symbol",
            keyValue: "A",
            column: "Factor",
            value: "{\"Numerator\":1,\"Denominator\":1}");

        migrationBuilder.UpdateData(
            table: "Units",
            keyColumn: "Symbol",
            keyValue: "cd",
            column: "Factor",
            value: "{\"Numerator\":1,\"Denominator\":1}");

        migrationBuilder.UpdateData(
            table: "Units",
            keyColumn: "Symbol",
            keyValue: "K",
            column: "Factor",
            value: "{\"Numerator\":1,\"Denominator\":1}");

        migrationBuilder.UpdateData(
            table: "Units",
            keyColumn: "Symbol",
            keyValue: "kg",
            column: "Factor",
            value: "{\"Numerator\":1,\"Denominator\":1}");

        migrationBuilder.UpdateData(
            table: "Units",
            keyColumn: "Symbol",
            keyValue: "m",
            column: "Factor",
            value: "{\"Numerator\":1,\"Denominator\":1}");

        migrationBuilder.UpdateData(
            table: "Units",
            keyColumn: "Symbol",
            keyValue: "mol",
            column: "Factor",
            value: "{\"Numerator\":1,\"Denominator\":1}");

        migrationBuilder.UpdateData(
            table: "Units",
            keyColumn: "Symbol",
            keyValue: "s",
            column: "Factor",
            value: "{\"Numerator\":1,\"Denominator\":1}");
    }
}
