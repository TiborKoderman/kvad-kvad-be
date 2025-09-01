// Infrastructure/Postgres/PostgresTypeMappings.cs
using Npgsql;
using kvad_be.Extensions.PostgresComposite;

public static class PostgresTypeMappings
{
  public static void Apply(NpgsqlDataSourceBuilder dsb)
  {
    // Use our new extension method to automatically map all composite types
    dsb.MapAllCompositeTypes();
    
    // Or map individual types (legacy approach for compatibility)
    // dsb.MapComposite<Dim7>("dim7");
    // dsb.MapComposite<Rational>("rational");
  }
}