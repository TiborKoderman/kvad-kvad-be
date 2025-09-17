using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace kvad_be.Database.Converters;


public class VectorArrayConverter : ValueConverter<Vector<short>, short[]>
{
    public VectorArrayConverter()
        : base(
            v => 
              short[] array = new short[Vector<short>.Count];
              v.CopyTo(array);
              return array;
            ,
            a => new Vector<short>(a)
        )
    { }
}
