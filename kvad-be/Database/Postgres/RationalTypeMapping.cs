using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

public sealed class RationalTypeMapping : RelationalTypeMapping
{
    public RationalTypeMapping()
        : base(new RelationalTypeMappingParameters(
            new CoreTypeMappingParameters(typeof(Rational), null, new RationalValueComparer()),
            storeType: "rational",
            dbType: null,
            unicode: false,
            size: null,
            fixedLength: false,
            precision: null,
            scale: null,
            storeTypePostfix: StoreTypePostfix.None))
    { }

  protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
  {
    return new RationalTypeMapping();
  }

  protected override string GenerateNonNullSqlLiteral(object value)
    {
        var r = (Rational)value;
        // Emit: ROW(<num>,<den>)::rational
        return $"ROW({r.Numerator},{r.Denominator})::rational";
    }

  // Optional but nice for parameters: send as text in Postgres's record text form "(a,b)"
  public Expression ComposeUsingCollation(Expression valueExpression, string collation)
  {
    return valueExpression;
  }

  private sealed class RationalValueComparer : ValueComparer<Rational>
    {
        public RationalValueComparer()
            : base(
                (a, b) => a.Numerator == b.Numerator && a.Denominator == b.Denominator,
                v => HashCode.Combine(v.Numerator, v.Denominator))
        { }
    }
}