using System.Numerics;

public static class VectorHelper
{
    // Standard dimension count for SI units (7 base dimensions)
    public const int StandardDimensionCount = 7;
    
    // Create a Vector<short> from an array, padding or truncating as needed
    public static Vector<short> CreateDimensionVector(params short[] values)
    {
        var array = new short[Vector<short>.Count];
        int copyCount = Math.Min(values.Length, Vector<short>.Count);
        Array.Copy(values, array, copyCount);
        return new Vector<short>(array);
    }
    
    // Create a zero dimension vector
    public static Vector<short> ZeroDimension() => Vector<short>.Zero;
    
    // Get a short array from Vector<short> for the first 7 elements
    public static short[] ToShortArray(this Vector<short> vector)
    {
        var result = new short[StandardDimensionCount];
        for (int i = 0; i < StandardDimensionCount && i < Vector<short>.Count; i++)
        {
            result[i] = vector[i];
        }
        return result;
    }
    
    // Get the effective length for dimension vectors (always 7 for SI units)
    public static int GetDimensionLength(this Vector<short> vector) => StandardDimensionCount;
    
    // Get element at index with bounds checking
    public static short GetDimension(this Vector<short> vector, int index)
    {
        if (index < 0 || index >= StandardDimensionCount || index >= Vector<short>.Count)
            return 0;
        return vector[index];
    }
    
    // Create a new vector with a specific dimension set
    public static Vector<short> SetDimension(this Vector<short> vector, int index, short value)
    {
        if (index < 0 || index >= StandardDimensionCount || index >= Vector<short>.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
            
        var array = vector.ToShortArray();
        array[index] = value;
        return CreateDimensionVector(array);
    }
    
    // Add dimensions element-wise (for combining unit dimensions)
    public static Vector<short> AddDimensions(this Vector<short> vector1, Vector<short> vector2, int exponent = 1)
    {
        var result = new short[StandardDimensionCount];
        for (int i = 0; i < StandardDimensionCount; i++)
        {
            result[i] = (short)(vector1.GetDimension(i) + (vector2.GetDimension(i) * exponent));
        }
        return CreateDimensionVector(result);
    }
}