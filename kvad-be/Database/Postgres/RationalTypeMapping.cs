using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Data.Common;
using System.Reflection;
using Npgsql;

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

    protected override void ConfigureParameter(DbParameter parameter)
    {
        base.ConfigureParameter(parameter);
        if (parameter is NpgsqlParameter npgParam)
        {
            npgParam.DataTypeName = "rational";
        }
    }

    public override Expression GenerateCodeLiteral(object value)
    {
        var r = (Rational)value;
        var ctor = typeof(Rational).GetConstructor(new[] { typeof(long), typeof(long) })!;
        return Expression.New(ctor, Expression.Constant(r.Numerator), Expression.Constant(r.Denominator));
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