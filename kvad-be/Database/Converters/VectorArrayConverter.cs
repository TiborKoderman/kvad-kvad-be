using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace kvad_be.Database.Converters;


public class VectorArrayConverter : ValueConverter<Vector<short>, short[]>
{
  public VectorArrayConverter()
      : base(
          v => ToArray(v),

          a => new Vector<short>(a)
      )
  { }


  private static short[] ToArray(Vector<short> vector)
  {
    short[] array = new short[Vector<short>.Count];
    vector.CopyTo(array);
    return array;
  }
    

}
