using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("00000000000000_CreatePgTypes")]
public partial class CreatePgTypes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'dim7') THEN
                    CREATE TYPE dim7 AS (
                        L SMALLINT,
                        M SMALLINT,
                        T SMALLINT,
                        I SMALLINT,
                        Th SMALLINT,
                        N SMALLINT,
                        J SMALLINT
                    );
                END IF;
            END
            $$;
        ");

        migrationBuilder.Sql(@"
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'rational') THEN
                    CREATE TYPE rational AS (
                        Numerator BIGINT,
                        Denominator BIGINT
                    );
                END IF;
            END
            $$;
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TYPE IF EXISTS dim7;");
        migrationBuilder.Sql("DROP TYPE IF EXISTS rational;");
    }
}