using kvad_be.Database;
using Microsoft.EntityFrameworkCore.Migrations;

[Microsoft.EntityFrameworkCore.Infrastructure.DbContext(typeof(AppDbContext))]
[Migration("99999999999998_CreateRationalArrayLengthConstraints")]
public partial class CreateRationalArrayLengthConstraints : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.EnsureSchema("public");
    
    // Add check constraints to ensure all Rational arrays (bigint[]) have exactly 2 elements
    
    // Units table - Factor column (Rational)
    migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" 
            ADD CONSTRAINT ck_units_factor_rational_length 
            CHECK (array_length(""Factor"", 1) = 2);
        ");

    // Units table - LogK column (Rational, nullable)
    migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" 
            ADD CONSTRAINT ck_units_logk_rational_length 
            CHECK (""LogK"" IS NULL OR array_length(""LogK"", 1) = 2);
        ");

    // Units table - LogRef column (Rational, nullable)
    migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" 
            ADD CONSTRAINT ck_units_logref_rational_length 
            CHECK (""LogRef"" IS NULL OR array_length(""LogRef"", 1) = 2);
        ");

    // UnitCanonicalPart table - Exponent column (Rational)
    migrationBuilder.Sql(@"
            ALTER TABLE ""UnitCanonicalPart"" 
            ADD CONSTRAINT ck_unitcanonicalpart_exponent_rational_length 
            CHECK (array_length(""Exponent"", 1) = 2);
        ");

    // Add additional check constraints for denominator != 0 (Rational validity)
    
    // Units table - Factor denominator check
    migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" 
            ADD CONSTRAINT ck_units_factor_denominator_nonzero 
            CHECK (""Factor""[2] != 0);
        ");

    // Units table - LogK denominator check (if not null)
    migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" 
            ADD CONSTRAINT ck_units_logk_denominator_nonzero 
            CHECK (""LogK"" IS NULL OR ""LogK""[2] != 0);
        ");

    // Units table - LogRef denominator check (if not null)
    migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" 
            ADD CONSTRAINT ck_units_logref_denominator_nonzero 
            CHECK (""LogRef"" IS NULL OR ""LogRef""[2] != 0);
        ");

    // UnitCanonicalPart table - Exponent denominator check
    migrationBuilder.Sql(@"
            ALTER TABLE ""UnitCanonicalPart"" 
            ADD CONSTRAINT ck_unitcanonicalpart_exponent_denominator_nonzero 
            CHECK (""Exponent""[2] != 0);
        ");
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    // Drop all Rational-related check constraints
    
    // Array length constraints
    migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" DROP CONSTRAINT IF EXISTS ck_units_factor_rational_length;
            ALTER TABLE ""Units"" DROP CONSTRAINT IF EXISTS ck_units_logk_rational_length;
            ALTER TABLE ""Units"" DROP CONSTRAINT IF EXISTS ck_units_logref_rational_length;
            ALTER TABLE ""UnitCanonicalPart"" DROP CONSTRAINT IF EXISTS ck_unitcanonicalpart_exponent_rational_length;
        ");

    // Denominator non-zero constraints
    migrationBuilder.Sql(@"
            ALTER TABLE ""Units"" DROP CONSTRAINT IF EXISTS ck_units_factor_denominator_nonzero;
            ALTER TABLE ""Units"" DROP CONSTRAINT IF EXISTS ck_units_logk_denominator_nonzero;
            ALTER TABLE ""Units"" DROP CONSTRAINT IF EXISTS ck_units_logref_denominator_nonzero;
            ALTER TABLE ""UnitCanonicalPart"" DROP CONSTRAINT IF EXISTS ck_unitcanonicalpart_exponent_denominator_nonzero;
        ");
  }
}