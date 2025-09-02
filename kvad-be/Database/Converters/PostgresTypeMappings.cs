// Infrastructure/Postgres/PostgresTypeMappings.cs
using Npgsql;

public static class PostgresTypeMappings
{
  public static void Apply(NpgsqlDataSourceBuilder dsb)
  {
    // Use explicit mapping for now to avoid design-time issues
    dsb.MapComposite<Dim7>("dim7");
    dsb.MapComposite<Rational>("rational");
    
    // TODO: Use automatic mapping once design-time issues are resolved
    // dsb.MapAllCompositeTypes();
  }
}