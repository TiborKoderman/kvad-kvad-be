// Infrastructure/Postgres/PostgresTypeMappings.cs
using Npgsql;

public static class PostgresTypeMappings
{
  public static void Apply(NpgsqlDataSourceBuilder dsb)
  {
    dsb.MapComposite<Dim7>("dim7");
    dsb.MapComposite<Rational>("rational");
  }
}