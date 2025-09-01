
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;

sealed class PgCompositeRelationalTypeMapping : RelationalTypeMapping
{
  readonly Type _clr;

  public PgCompositeRelationalTypeMapping(Type clr, string storeType)
      : base(new RelationalTypeMappingParameters(new CoreTypeMappingParameters(clr), storeType))
      => _clr = clr;

  protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters p)
      => new PgCompositeRelationalTypeMapping(_clr, p.StoreType);

  // Tell Npgsql which PG type this parameter is
  protected override void ConfigureParameter(DbParameter parameter)
  {
    if (parameter is NpgsqlParameter npg)
      npg.DataTypeName = StoreType; // e.g. "public.dim7"
    base.ConfigureParameter(parameter);
  }
}