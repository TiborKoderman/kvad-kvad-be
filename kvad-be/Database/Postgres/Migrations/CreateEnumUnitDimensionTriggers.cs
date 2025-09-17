using kvad_be.Database;
using Microsoft.EntityFrameworkCore.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(AppDbContext))]
[Migration("99999999999999_CreateEnumUnitDimensionTriggers")]
public partial class CreateEnumUnitDimensionTriggers : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.EnsureSchema("public");
    
    // 1. PostgreSQL function to validate sequential IDs on insert
    migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION validate_enumunitdimension_sequential_id()
            RETURNS TRIGGER AS $$
            BEGIN
                -- Allow first record with Id = 0
                IF NEW.""Id"" = 0 THEN
                    -- Ensure no other record exists if we're inserting Id = 0
                    IF EXISTS (SELECT 1 FROM ""EnumUnitDimensions"" WHERE ""Id"" = 0) THEN
                        RAISE EXCEPTION 'EnumUnitDimension Id 0 already exists';
                    END IF;
                    RETURN NEW;
                END IF;
                
                -- For any Id > 0, ensure previous Id exists (no gaps)
                IF NOT EXISTS (SELECT 1 FROM ""EnumUnitDimensions"" WHERE ""Id"" = NEW.""Id"" - 1) THEN
                    RAISE EXCEPTION 'EnumUnitDimension Id must be sequential - Id % does not exist', NEW.""Id"" - 1;
                END IF;
                
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ");

    // 2. PostgreSQL function to prevent deletion of EnumUnitDimension if used in any Unit
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

    // 2. PostgreSQL function to expand Unit.Dimension arrays when new EnumUnitDimension is added
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

    // 3. PostgreSQL function to automatically pad Unit.Dimension arrays to correct length
    migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION pad_unit_dimension_array()
            RETURNS TRIGGER AS $$
            DECLARE
                expected_length integer;
                actual_length integer;
                padded_array smallint[];
            BEGIN
                -- Get expected length from EnumUnitDimensions count
                SELECT COUNT(*) INTO expected_length FROM ""EnumUnitDimensions"";
                
                -- Get actual length of the Dimension array
                actual_length := array_length(NEW.""Dimension"", 1);
                
                -- If array is null or empty, create a zero-filled array of correct length
                IF actual_length IS NULL THEN
                    NEW.""Dimension"" := array_fill(0::smallint, ARRAY[expected_length]);
                -- If array is shorter than expected, pad with zeros
                ELSIF actual_length < expected_length THEN
                    -- Create padding array of zeros
                    padded_array := array_fill(0::smallint, ARRAY[expected_length - actual_length]);
                    -- Concatenate original array with padding
                    NEW.""Dimension"" := NEW.""Dimension"" || padded_array;
                -- If array is longer than expected, truncate it
                ELSIF actual_length > expected_length THEN
                    NEW.""Dimension"" := NEW.""Dimension""[1:expected_length];
                END IF;
                
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ");

    // 4. Create trigger to validate sequential IDs on EnumUnitDimension insert
    migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_enumunitdimension_validate_sequential_id ON ""EnumUnitDimensions"";
            CREATE TRIGGER trg_enumunitdimension_validate_sequential_id
                BEFORE INSERT ON ""EnumUnitDimensions""
                FOR EACH ROW
                EXECUTE FUNCTION validate_enumunitdimension_sequential_id();
        ");

    // 5. Create trigger to prevent deletion of EnumUnitDimension if used
    migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_enumunitdimension_prevent_delete_if_used ON ""EnumUnitDimensions"";
            CREATE TRIGGER trg_enumunitdimension_prevent_delete_if_used
                BEFORE DELETE ON ""EnumUnitDimensions""
                FOR EACH ROW
                EXECUTE FUNCTION prevent_enumunitdimension_delete_if_used();
        ");

    // 5. Create trigger to expand Unit dimensions when new EnumUnitDimension is added
    migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_enumunitdimension_expand_unit_dimensions ON ""EnumUnitDimensions"";
            CREATE TRIGGER trg_enumunitdimension_expand_unit_dimensions
                AFTER INSERT ON ""EnumUnitDimensions""
                FOR EACH ROW
                EXECUTE FUNCTION expand_unit_dimensions_on_new_enum();
        ");

    // 6. Create trigger to automatically pad Unit dimension arrays
    migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_unit_pad_dimension_array ON ""Units"";
            CREATE TRIGGER trg_unit_pad_dimension_array
                BEFORE INSERT OR UPDATE ON ""Units""
                FOR EACH ROW
                EXECUTE FUNCTION pad_unit_dimension_array();
        ");
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    // Drop triggers first
    migrationBuilder.Sql(@"
            DROP TRIGGER IF EXISTS trg_enumunitdimension_validate_sequential_id ON ""EnumUnitDimensions"";
            DROP TRIGGER IF EXISTS trg_enumunitdimension_prevent_delete_if_used ON ""EnumUnitDimensions"";
            DROP TRIGGER IF EXISTS trg_enumunitdimension_expand_unit_dimensions ON ""EnumUnitDimensions"";
            DROP TRIGGER IF EXISTS trg_unit_pad_dimension_array ON ""Units"";
        ");

    // Drop functions
    migrationBuilder.Sql(@"
            DROP FUNCTION IF EXISTS validate_enumunitdimension_sequential_id();
            DROP FUNCTION IF EXISTS prevent_enumunitdimension_delete_if_used();
            DROP FUNCTION IF EXISTS expand_unit_dimensions_on_new_enum();
            DROP FUNCTION IF EXISTS pad_unit_dimension_array();
        ");
  }
}