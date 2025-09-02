// using System;
// using System.Data.Common;
// using System.Linq.Expressions;
// using System.Reflection;
// using Microsoft.EntityFrameworkCore.Storage;
// using Npgsql;

// sealed class Dim7TypeMapping : RelationalTypeMapping
// {
//     public Dim7TypeMapping() : base(
//         new RelationalTypeMappingParameters(
//             new CoreTypeMappingParameters(typeof(Dim7)),
//             storeType: "dim7"))
//     { }

//     protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters p)
//         => new Dim7TypeMapping();

//     protected override void ConfigureParameter(DbParameter parameter)
//     {
//         base.ConfigureParameter(parameter);
//         if (parameter is NpgsqlParameter npg) npg.DataTypeName = StoreType;
//     }

//     // <-- This fixes your exception:
//     public override Expression GenerateCodeLiteral(object? value)
//     {
//         if (value is null) return Expression.Constant(null, typeof(Dim7));
//         var d = (Dim7)value;

//         var ctor = typeof(Dim7).GetConstructor(new[]
//         {
//             typeof(short), typeof(short), typeof(short), typeof(short),
//             typeof(short), typeof(short), typeof(short)
//         })!;

//         return Expression.New(ctor,
//             Expression.Constant(d.L),
//             Expression.Constant(d.M),
//             Expression.Constant(d.T),
//             Expression.Constant(d.I),
//             Expression.Constant(d.Th),
//             Expression.Constant(d.N),
//             Expression.Constant(d.J));
//     }
// }