using System.Data.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.Reflection;
using System.Linq.Expressions;

sealed class PgCompositeTypeMapping<T> : RelationalTypeMapping
{
    public PgCompositeTypeMapping(string storeType)
        : base(new RelationalTypeMappingParameters(
            new CoreTypeMappingParameters(typeof(T)),
            storeType))
    { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
        => new PgCompositeTypeMapping<T>(parameters.StoreType);

    // Ensure provider parameter is NpgsqlParameter and carries the composite type name.
    protected override void ConfigureParameter(DbParameter parameter)
    {
        base.ConfigureParameter(parameter);
        if (parameter is NpgsqlParameter npg)
        {
            // Key line: tell Npgsql which composite this is
            npg.DataTypeName = StoreType; // e.g., "rational"
            // Optional: npg.NpgsqlDbType = NpgsqlDbType.Unknown; // not required if DataTypeName is set
        }
    }

    // How EF reads the value from DbDataReader
    public override MethodInfo GetDataReaderMethod()
        => typeof(DbDataReader).GetMethod(nameof(DbDataReader.GetFieldValue))!
                               .MakeGenericMethod(typeof(T));

    public override Expression GenerateCodeLiteral(object value)
        => throw new NotSupportedException("EF code literals for composites aren’t supported.");
}
