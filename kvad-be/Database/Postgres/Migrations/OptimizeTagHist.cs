using kvad_be.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("99999999999999_OptimizeTagHist")]
public partial class OptimizeTagHist : Migration
{
  protected override void Up(MigrationBuilder migrationBuilder)
  {

    //Toast compress
    migrationBuilder.Sql(@"
    DO $$
    BEGIN
      IF current_setting('server_version_num')::int >= 140000 THEN
        -- Use LZ4 for wide/TOASTed columns
        EXECUTE 'ALTER TABLE public.""TagHists"" ALTER COLUMN ""V_string"" SET COMPRESSION lz4';
        EXECUTE 'ALTER TABLE public.""TagHists"" ALTER COLUMN ""V_json""   SET COMPRESSION lz4';
        EXECUTE 'ALTER TABLE public.""TagHists"" ALTER COLUMN ""V_bytea""  SET COMPRESSION lz4';
      END IF;
    END $$;
    ");

    migrationBuilder.Sql(@"
      select create_hypertable('public.""TagHists""',
      'Ts',
      partitioning_column => 'TagId',
      number_partitions => 8,
      if_not_exists => true
      );
        ");

    // Pick a chunk time interval that gives ~0.5–2 GB per chunk at your ingest rate
    migrationBuilder.Sql(@"SELECT set_chunk_time_interval('public.""TagHists""', INTERVAL '7 days');");


    migrationBuilder.Sql(@"
        ALTER TABLE public.""TagHists"" SET (
          timescaledb.compress,
          timescaledb.compress_segmentby = '""TagId""',
          timescaledb.compress_orderby   = '""Ts"" DESC'
        );
        ");
        
        migrationBuilder.Sql(@"SELECT add_compression_policy('public.""TagHists""', INTERVAL '7 days');");
  }
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.Sql(@"SELECT remove_compression_policy('public.""TagHists""') WHERE EXISTS (SELECT 1);");
    migrationBuilder.Sql(@"SELECT remove_retention_policy('public.""TagHists""') WHERE EXISTS (SELECT 1);");
  }
}