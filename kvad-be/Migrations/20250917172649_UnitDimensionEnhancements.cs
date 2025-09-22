using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable
/// <inheritdoc />
public partial class UnitDimensionEnhancements : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(
            name: "ck_unit_dimension_array_length",
            table: "Units");

        migrationBuilder.DropPrimaryKey(
            name: "PK_UnitDimensions",
            table: "UnitDimensions");

        migrationBuilder.DropCheckConstraint(
            name: "ck_enumunitdimension_sequential_ids",
            table: "UnitDimensions");

        migrationBuilder.DeleteData(
            table: "UnitDimensions",
            keyColumn: "Id",
            keyValue: 7);

        migrationBuilder.RenameTable(
            name: "UnitDimensions",
            newName: "EnumUnitDimensions");

        migrationBuilder.RenameIndex(
            name: "IX_UnitDimensions_Symbol",
            table: "EnumUnitDimensions",
            newName: "IX_EnumUnitDimensions_Symbol");

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "EnumUnitDimensions",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer")
            .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

        migrationBuilder.AddPrimaryKey(
            name: "PK_EnumUnitDimensions",
            table: "EnumUnitDimensions",
            column: "Id");

        migrationBuilder.UpdateData(
            table: "EnumUnitDimensions",
            keyColumn: "Id",
            keyValue: 1,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Mass", "M" });

        migrationBuilder.UpdateData(
            table: "EnumUnitDimensions",
            keyColumn: "Id",
            keyValue: 2,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Time", "T" });

        migrationBuilder.UpdateData(
            table: "EnumUnitDimensions",
            keyColumn: "Id",
            keyValue: 3,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Electric Current", "I" });

        migrationBuilder.UpdateData(
            table: "EnumUnitDimensions",
            keyColumn: "Id",
            keyValue: 4,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Thermodynamic Temperature", "Θ" });

        migrationBuilder.UpdateData(
            table: "EnumUnitDimensions",
            keyColumn: "Id",
            keyValue: 5,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Amount of Substance", "N" });

        migrationBuilder.UpdateData(
            table: "EnumUnitDimensions",
            keyColumn: "Id",
            keyValue: 6,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Luminous Intensity", "J" });

        migrationBuilder.InsertData(
            table: "EnumUnitDimensions",
            columns: new[] { "Id", "Name", "Symbol" },
            values: new object[] { 0, "Length", "L" });

        // Add constraints and triggers for EnumUnitDimension management
        
        // 1. Constraint to ensure sequential IDs starting from 0
        migrationBuilder.AddCheckConstraint(
            name: "ck_enumunitdimension_sequential_ids",
            table: "EnumUnitDimensions",
            sql: "(\"Id\" = 0) OR EXISTS (SELECT 1 FROM \"EnumUnitDimensions\" e WHERE e.\"Id\" = \"EnumUnitDimensions\".\"Id\" - 1)");

        // 2. Constraint to ensure Unit.Dimension array length matches EnumUnitDimension count
        migrationBuilder.AddCheckConstraint(
            name: "ck_unit_dimension_array_length",
            table: "Units",
            sql: "array_length(\"Dimension\", 1) = (SELECT COUNT(*) FROM \"EnumUnitDimensions\")");

        // 3. PostgreSQL function to prevent deletion of EnumUnitDimension if used in any Unit
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION prevent_enumunitdimension_delete_if_used()
            RETURNS TRIGGER AS $$
            BEGIN
                -- Check if any Unit has a non-zero value at the dimension index (Id directly matches array index since both start from 0)
                IF EXISTS (
                    SELECT 1 FROM ""Units"" 
                    WHERE ""Dimension""[OLD.""Id"" + 1] != 0  -- PostgreSQL arrays are 1-indexed, so add 1
                ) THEN
                    RAISE EXCEPTION 'Cannot delete EnumUnitDimension with Id % (%) because it is used in at least one Unit''s Dimension array', 
                        OLD.""Id"", OLD.""Symbol"";
                END IF;
                
                RETURN OLD;
            END;
            $$ LANGUAGE plpgsql;
        ");

        // 4. PostgreSQL function to expand Unit.Dimension arrays when new EnumUnitDimension is added
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION expand_unit_dimensions_on_new_enum()
            RETURNS TRIGGER AS $$
            BEGIN
                -- Add a 0 at the end of all existing Unit.Dimension arrays
                -- This appends a new element to match the new EnumUnitDimension count
                UPDATE ""Units"" 
                SET ""Dimension"" = ""Dimension"" || ARRAY[0]::smallint[];
                
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ");

        // 5. PostgreSQL function to validate new Unit insertions have correct dimension array length
        migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION validate_unit_dimension_length()
            RETURNS TRIGGER AS $$
            DECLARE
                expected_length integer;
                actual_length integer;
            BEGIN
                -- Get expected length from EnumUnitDimensions count
                SELECT COUNT(*) INTO expected_length FROM ""EnumUnitDimensions"";
                
                -- Get actual length of the Dimension array
                actual_length := array_length(NEW.""Dimension"", 1);
                
                -- Validate length
                IF actual_length != expected_length THEN
                    RAISE EXCEPTION 'Unit Dimension array length (%) does not match EnumUnitDimensions count (%). Array must have exactly % elements.',
                        actual_length, expected_length, expected_length;
                END IF;
                
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ");

        // 6. Create triggers
        migrationBuilder.Sql(@"
            CREATE TRIGGER trg_enumunitdimension_prevent_delete_if_used
                BEFORE DELETE ON ""EnumUnitDimensions""
                FOR EACH ROW
                EXECUTE FUNCTION prevent_enumunitdimension_delete_if_used();
        ");

        migrationBuilder.Sql(@"
            CREATE TRIGGER trg_enumunitdimension_expand_unit_dimensions
                AFTER INSERT ON ""EnumUnitDimensions""
                FOR EACH ROW
                EXECUTE FUNCTION expand_unit_dimensions_on_new_enum();
        ");

        migrationBuilder.Sql(@"
            CREATE TRIGGER trg_unit_validate_dimension_length
                BEFORE INSERT OR UPDATE ON ""Units""
                FOR EACH ROW
                EXECUTE FUNCTION validate_unit_dimension_length();
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop triggers first
        migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_enumunitdimension_prevent_delete_if_used ON \"EnumUnitDimensions\";");
        migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_enumunitdimension_expand_unit_dimensions ON \"EnumUnitDimensions\";");
        migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_unit_validate_dimension_length ON \"Units\";");

        // Drop functions
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS prevent_enumunitdimension_delete_if_used();");
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS expand_unit_dimensions_on_new_enum();");
        migrationBuilder.Sql("DROP FUNCTION IF EXISTS validate_unit_dimension_length();");

        // Drop constraints
        migrationBuilder.DropCheckConstraint(
            name: "ck_enumunitdimension_sequential_ids",
            table: "EnumUnitDimensions");

        migrationBuilder.DropCheckConstraint(
            name: "ck_unit_dimension_array_length",
            table: "Units");
        migrationBuilder.DropPrimaryKey(
            name: "PK_EnumUnitDimensions",
            table: "EnumUnitDimensions");

        migrationBuilder.DeleteData(
            table: "EnumUnitDimensions",
            keyColumn: "Id",
            keyValue: 0);

        migrationBuilder.RenameTable(
            name: "EnumUnitDimensions",
            newName: "UnitDimensions");

        migrationBuilder.RenameIndex(
            name: "IX_EnumUnitDimensions_Symbol",
            table: "UnitDimensions",
            newName: "IX_UnitDimensions_Symbol");

        migrationBuilder.AlterColumn<int>(
            name: "Id",
            table: "UnitDimensions",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer")
            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

        migrationBuilder.AddPrimaryKey(
            name: "PK_UnitDimensions",
            table: "UnitDimensions",
            column: "Id");

        migrationBuilder.UpdateData(
            table: "UnitDimensions",
            keyColumn: "Id",
            keyValue: 1,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Length", "L" });

        migrationBuilder.UpdateData(
            table: "UnitDimensions",
            keyColumn: "Id",
            keyValue: 2,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Mass", "M" });

        migrationBuilder.UpdateData(
            table: "UnitDimensions",
            keyColumn: "Id",
            keyValue: 3,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Time", "T" });

        migrationBuilder.UpdateData(
            table: "UnitDimensions",
            keyColumn: "Id",
            keyValue: 4,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Electric Current", "I" });

        migrationBuilder.UpdateData(
            table: "UnitDimensions",
            keyColumn: "Id",
            keyValue: 5,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Thermodynamic Temperature", "Θ" });

        migrationBuilder.UpdateData(
            table: "UnitDimensions",
            keyColumn: "Id",
            keyValue: 6,
            columns: new[] { "Name", "Symbol" },
            values: new object[] { "Amount of Substance", "N" });

        migrationBuilder.InsertData(
            table: "UnitDimensions",
            columns: new[] { "Id", "Name", "Symbol" },
            values: new object[] { 7, "Luminous Intensity", "J" });

        migrationBuilder.AddCheckConstraint(
            name: "ck_unit_dimension_array_length",
            table: "Units",
            sql: "array_length(\"Dimension\", 1) = (SELECT COUNT(*) FROM \"EnumUnitDimensions\")");

        migrationBuilder.AddCheckConstraint(
            name: "ck_enumunitdimension_sequential_ids",
            table: "UnitDimensions",
            sql: "(\"Id\" = 1) OR EXISTS (SELECT 1 FROM \"EnumUnitDimensions\" e WHERE e.\"Id\" = \"EnumUnitDimensions\".\"Id\" - 1)");
    }
}
