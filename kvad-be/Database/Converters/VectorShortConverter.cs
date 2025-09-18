using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Numerics;

namespace kvad_be.Database.Converters
{
    /// <summary>
    /// Converts Vector<short> to/from short[] for Entity Framework storage
    /// </summary>
    public class VectorShortConverter : ValueConverter<Vector<short>, short[]>
    {
        public VectorShortConverter() : base(
            v => ConvertToArray(v),
            v => ConvertToVector(v))
        {
        }

        private static short[] ConvertToArray(Vector<short> vector)
        {
            return vector.ToShortArray();
        }

        private static Vector<short> ConvertToVector(short[] array)
        {
            return VectorHelper.CreateDimensionVector(array ?? new short[VectorHelper.StandardDimensionCount]);
        }
    }
}