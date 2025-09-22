using Microsoft.EntityFrameworkCore.Migrations;using Microsoft.EntityF            constraints: table =>

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;            {

                table.PrimaryKey("PK_UnitDimensions", x => x.Id);

#nullable disable                table.CheckConstraint("ck_enumunitdimension_sequential_ids", 

                    "(\"Id\" = 0) OR EXISTS (SELECT 1 FROM \"UnitDimensions\" e WHERE e.\"Id\" = \"UnitDimensions\".\"Id\" - 1)");

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional            });rkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

/// <inheritdoc />

public partial class EnumUnitDimensionConstraintsAndTriggers : Migration#nullable disable

{

    /// <inheritdoc />#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

    protected override void Up(MigrationBuilder migrationBuilder)

    {/// <inheritdoc />

        // Create UnitDimensions tablepublic partial class EnumUnitDimensionConstraintsAndTriggers : Migration

        migrationBuilder.CreateTable({

            name: "UnitDimensions",    /// <inheritdoc />

            columns: table => new    protected override void Up(MigrationBuilder migrationBuilder)

            {    {

                Id = table.Column<int>(type: "integer", nullable: false)        // Create EnumUnitDimensions table

                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),        migrationBuilder.CreateTable(

                Symbol = table.Column<string>(type: "text", nullable: false),            name: "UnitDimensions",

                Name = table.Column<string>(type: "text", nullable: false)            columns: table => new

            },            {

            constraints: table =>                Id = table.Column<int>(type: "integer", nullable: false)

            {                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),

                table.PrimaryKey("PK_UnitDimensions", x => x.Id);                Symbol = table.Column<string>(type: "text", nullable: false),

                table.CheckConstraint("ck_enumunitdimension_sequential_ids",                 Name = table.Column<string>(type: "text", nullable: false)

                    "(\"Id\" = 0) OR EXISTS (SELECT 1 FROM \"UnitDimensions\" e WHERE e.\"Id\" = \"UnitDimensions\".\"Id\" - 1)");            },

            });            constraints: table =>

            {

        // Seed the 7 SI base dimensions starting from Id = 0 to match array indices                table.PrimaryKey("PK_UnitDimensions", x => x.Id);

        migrationBuilder.InsertData(                table.CheckConstraint("ck_enumunitdimension_sequential_ids", 

            table: "UnitDimensions",                    "(\"Id\" = 1) OR EXISTS (SELECT 1 FROM \"UnitDimensions\" e WHERE e.\"Id\" = \"UnitDimensions\".\"Id\" - 1)");

            columns: new[] { "Id", "Name", "Symbol" },            });

            values: new object[,]

            {        // Seed the 7 SI base dimensions starting from Id = 0 to match array indices

                { 0, "Length", "L" },        migrationBuilder.InsertData(

                { 1, "Mass", "M" },            table: "UnitDimensions",

                { 2, "Time", "T" },            columns: new[] { "Id", "Name", "Symbol" },

                { 3, "Electric Current", "I" },            values: new object[,]

                { 4, "Thermodynamic Temperature", "Θ" },            {

                { 5, "Amount of Substance", "N" },                { 0, "Length", "L" },

                { 6, "Luminous Intensity", "J" }                { 1, "Mass", "M" },

            });                { 2, "Time", "T" },

                { 3, "Electric Current", "I" },

        // Add constraint to ensure Unit.Dimension array length matches UnitDimensions count                { 4, "Thermodynamic Temperature", "Θ" },

        migrationBuilder.AddCheckConstraint(                { 5, "Amount of Substance", "N" },

            name: "ck_unit_dimension_array_length",                { 6, "Luminous Intensity", "J" }

            table: "Units",            });

            sql: "array_length(\"Dimension\", 1) = (SELECT COUNT(*) FROM \"UnitDimensions\")");

        // Add constraint to ensure Unit.Dimension array length matches EnumUnitDimension count

        // Create unique index on Symbol        migrationBuilder.AddCheckConstraint(

        migrationBuilder.CreateIndex(            name: "ck_unit_dimension_array_length",

            name: "IX_UnitDimensions_Symbol",            table: "Units",

            table: "UnitDimensions",            sql: "array_length(\"Dimension\", 1) = (SELECT COUNT(*) FROM \"UnitDimensions\")");

            column: "Symbol",

            unique: true);        // Create unique index on Symbol

        migrationBuilder.CreateIndex(

        // Create PostgreSQL function to prevent deletion of UnitDimension if used in any Unit            name: "IX_UnitDimensions_Symbol",

        migrationBuilder.Sql(@"            table: "UnitDimensions",

            CREATE OR REPLACE FUNCTION prevent_unitdimension_delete_if_used()            column: "Symbol",

            RETURNS TRIGGER AS $$            unique: true);

            BEGIN

                -- Check if any Unit has a non-zero value at the dimension index (Id directly, since IDs start from 0)        // Create PostgreSQL function to prevent deletion of EnumUnitDimension if used in any Unit

                IF EXISTS (        migrationBuilder.Sql(@"

                    SELECT 1 FROM ""Units""             CREATE OR REPLACE FUNCTION prevent_enumunitdimension_delete_if_used()

                    WHERE ""Dimension""[OLD.""Id"" + 1] != 0  -- PostgreSQL arrays are 1-indexed, so add 1            RETURNS TRIGGER AS $$

                ) THEN            BEGIN

                    RAISE EXCEPTION 'Cannot delete UnitDimension with Id % (%) because it is used in at least one Unit''s Dimension array',                 -- Check if any Unit has a non-zero value at the dimension index (Id-1, since arrays are 0-indexed)

                        OLD.""Id"", OLD.""Symbol"";                IF EXISTS (

                END IF;                    SELECT 1 FROM ""Units"" 

                                    WHERE ""Dimension""[OLD.""Id""] != 0

                RETURN OLD;                ) THEN

            END;                    RAISE EXCEPTION 'Cannot delete EnumUnitDimension with Id % (%) because it is used in at least one Unit''s Dimension array', 

            $$ LANGUAGE plpgsql;                        OLD.""Id"", OLD.""Symbol"";

        ");                END IF;

                

        // Create PostgreSQL function to expand Unit.Dimension arrays when new UnitDimension is added                RETURN OLD;

        migrationBuilder.Sql(@"            END;

            CREATE OR REPLACE FUNCTION expand_unit_dimensions_on_new_enum()            $$ LANGUAGE plpgsql;

            RETURNS TRIGGER AS $$        ");

            BEGIN

                -- Add a 0 at the end of all existing Unit.Dimension arrays        // Create PostgreSQL function to expand Unit.Dimension arrays when new EnumUnitDimension is added

                -- This appends a new element to match the new UnitDimension count        migrationBuilder.Sql(@"

                UPDATE ""Units""             CREATE OR REPLACE FUNCTION expand_unit_dimensions_on_new_enum()

                SET ""Dimension"" = ""Dimension"" || ARRAY[0]::smallint[];            RETURNS TRIGGER AS $$

                            BEGIN

                RETURN NEW;                -- Add a 0 at the end of all existing Unit.Dimension arrays

            END;                -- This appends a new element to match the new EnumUnitDimension count

            $$ LANGUAGE plpgsql;                UPDATE ""Units"" 

        ");                SET ""Dimension"" = ""Dimension"" || ARRAY[0]::smallint[];

                

        // Create PostgreSQL function to validate new Unit insertions have correct dimension array length                RETURN NEW;

        migrationBuilder.Sql(@"            END;

            CREATE OR REPLACE FUNCTION validate_unit_dimension_length()            $$ LANGUAGE plpgsql;

            RETURNS TRIGGER AS $$        ");

            DECLARE

                expected_length integer;        // Create PostgreSQL function to validate new Unit insertions have correct dimension array length

                actual_length integer;        migrationBuilder.Sql(@"

            BEGIN            CREATE OR REPLACE FUNCTION validate_unit_dimension_length()

                -- Get expected length from UnitDimensions count            RETURNS TRIGGER AS $$

                SELECT COUNT(*) INTO expected_length FROM ""UnitDimensions"";            DECLARE

                                expected_length integer;

                -- Get actual length of the Dimension array                actual_length integer;

                actual_length := array_length(NEW.""Dimension"", 1);            BEGIN

                                -- Get expected length from UnitDimensions count

                -- Validate length                SELECT COUNT(*) INTO expected_length FROM ""UnitDimensions"";

                IF actual_length != expected_length THEN                

                    RAISE EXCEPTION 'Unit Dimension array length (%) does not match UnitDimensions count (%). Array must have exactly % elements.',                -- Get actual length of the Dimension array

                        actual_length, expected_length, expected_length;                actual_length := array_length(NEW.""Dimension"", 1);

                END IF;                

                                -- Validate length

                RETURN NEW;                IF actual_length != expected_length THEN

            END;                    RAISE EXCEPTION 'Unit Dimension array length (%) does not match UnitDimensions count (%). Array must have exactly % elements.',

            $$ LANGUAGE plpgsql;                        actual_length, expected_length, expected_length;

        ");                END IF;

                

        // Create triggers                RETURN NEW;

        migrationBuilder.Sql(@"            END;

            CREATE TRIGGER trg_unitdimension_prevent_delete_if_used            $$ LANGUAGE plpgsql;

                BEFORE DELETE ON ""UnitDimensions""        ");

                FOR EACH ROW

                EXECUTE FUNCTION prevent_unitdimension_delete_if_used();        // Create triggers

        ");        migrationBuilder.Sql(@"

            CREATE TRIGGER trg_enumunitdimension_prevent_delete_if_used

        migrationBuilder.Sql(@"                BEFORE DELETE ON ""UnitDimensions""

            CREATE TRIGGER trg_unitdimension_expand_unit_dimensions                FOR EACH ROW

                AFTER INSERT ON ""UnitDimensions""                EXECUTE FUNCTION prevent_enumunitdimension_delete_if_used();

                FOR EACH ROW        ");

                EXECUTE FUNCTION expand_unit_dimensions_on_new_enum();

        ");        migrationBuilder.Sql(@"

            CREATE TRIGGER trg_enumunitdimension_expand_unit_dimensions

        migrationBuilder.Sql(@"                AFTER INSERT ON ""UnitDimensions""

            CREATE TRIGGER trg_unit_validate_dimension_length                FOR EACH ROW

                BEFORE INSERT OR UPDATE ON ""Units""                EXECUTE FUNCTION expand_unit_dimensions_on_new_enum();

                FOR EACH ROW        ");

                EXECUTE FUNCTION validate_unit_dimension_length();

        ");        migrationBuilder.Sql(@"

    }            CREATE TRIGGER trg_unit_validate_dimension_length

                BEFORE INSERT OR UPDATE ON ""Units""

    /// <inheritdoc />                FOR EACH ROW

    protected override void Down(MigrationBuilder migrationBuilder)                EXECUTE FUNCTION validate_unit_dimension_length();

    {        ");

        // Drop triggers    }

        migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_unitdimension_prevent_delete_if_used ON \"UnitDimensions\";");

        migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_unitdimension_expand_unit_dimensions ON \"UnitDimensions\";");    /// <inheritdoc />

        migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_unit_validate_dimension_length ON \"Units\";");    protected override void Down(MigrationBuilder migrationBuilder)

    {

        // Drop functions        // Drop triggers

        migrationBuilder.Sql("DROP FUNCTION IF EXISTS prevent_unitdimension_delete_if_used();");        migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_enumunitdimension_prevent_delete_if_used ON \"UnitDimensions\";");

        migrationBuilder.Sql("DROP FUNCTION IF EXISTS expand_unit_dimensions_on_new_enum();");        migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_enumunitdimension_expand_unit_dimensions ON \"UnitDimensions\";");

        migrationBuilder.Sql("DROP FUNCTION IF EXISTS validate_unit_dimension_length();");        migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_unit_validate_dimension_length ON \"Units\";");



        // Drop constraint and table        // Drop functions

        migrationBuilder.DropCheckConstraint(        migrationBuilder.Sql("DROP FUNCTION IF EXISTS prevent_enumunitdimension_delete_if_used();");

            name: "ck_unit_dimension_array_length",        migrationBuilder.Sql("DROP FUNCTION IF EXISTS expand_unit_dimensions_on_new_enum();");

            table: "Units");        migrationBuilder.Sql("DROP FUNCTION IF EXISTS validate_unit_dimension_length();");



        migrationBuilder.DropTable(        // Drop constraint and table

            name: "UnitDimensions");        migrationBuilder.DropCheckConstraint(

    }            name: "ck_unit_dimension_array_length",

}            table: "Units");

        migrationBuilder.DropTable(
            name: "UnitDimensions");
    }
}
