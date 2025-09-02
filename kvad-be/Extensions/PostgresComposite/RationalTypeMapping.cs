using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;


sealed class RationalTypeMapping : RelationalTypeMapping
{
  public RationalTypeMapping() : base(
      new RelationalTypeMappingParameters(
          new CoreTypeMappingParameters(typeof(Rational)),
          storeType: "rational"))
  { }

  protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters p)
      => new RationalTypeMapping();

  protected override void ConfigureParameter(DbParameter parameter)
  {
    base.ConfigureParameter(parameter);
    if (parameter is NpgsqlParameter npg) npg.DataTypeName = StoreType;
  }

  public override Expression GenerateCodeLiteral(object value)
  {
    var r = (Rational)value;
    var ctor = typeof(Rational).GetConstructor(new[] { typeof(long), typeof(long) })!;
    return Expression.New(ctor, Expression.Constant(r.Numerator), Expression.Constant(r.Denominator));
  }
}