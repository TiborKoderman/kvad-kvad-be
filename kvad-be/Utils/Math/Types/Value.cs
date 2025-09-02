public struct Value<T>
{
    private readonly T _value;
    public Value(T value)
    {
        _value = value;
    }

    public static byte[] ToBytes(Value<T> value)
    {
        if (value._value is int)
            return BitConverter.GetBytes((int)(object)value._value);
        if (value._value is double)
            return BitConverter.GetBytes((double)(object)value._value);
        if (value._value is float)
            return BitConverter.GetBytes((float)(object)value._value);
        if (value._value is long)
            return BitConverter.GetBytes((long)(object)value._value);
        if (value._value is bool)
            return BitConverter.GetBytes((bool)(object)value._value);
        if (value._value is short)
            return BitConverter.GetBytes((short)(object)value._value);
        if (value._value is string)
            return System.Text.Encoding.UTF8.GetBytes((string)(object)value._value);
        throw new NotSupportedException($"Type {typeof(T)} is not supported for conversion to bytes.");
    }

    public static Value<T> FromBytes(byte[] bytes)
    {
        if (typeof(T) == typeof(int))
            return new Value<T>((T)(object)BitConverter.ToInt32(bytes, 0));
        if (typeof(T) == typeof(double))
            return new Value<T>((T)(object)BitConverter.ToDouble(bytes, 0));
        if (typeof(T) == typeof(float))
            return new Value<T>((T)(object)BitConverter.ToSingle(bytes, 0));
        if (typeof(T) == typeof(long))
            return new Value<T>((T)(object)BitConverter.ToInt64(bytes, 0));
        if (typeof(T) == typeof(bool))
            return new Value<T>((T)(object)BitConverter.ToBoolean(bytes, 0));
        if (typeof(T) == typeof(short))
            return new Value<T>((T)(object)BitConverter.ToInt16(bytes, 0));
        if (typeof(T) == typeof(string))
            return new Value<T>((T)(object)System.Text.Encoding.UTF8.GetString(bytes));
        throw new NotSupportedException($"Type {typeof(T)} is not supported for conversion from bytes.");
    }



}
