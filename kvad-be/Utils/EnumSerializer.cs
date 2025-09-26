
using System;

[AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = false)]
public sealed class Serde : Attribute
{
  /// <summary>
  /// Deserialize a string to an enum value of type T. Parsing is case-insensitive.
  /// Throws <see cref="ArgumentNullException"/> when <paramref name="value"/> is null
  /// and <see cref="ArgumentException"/> when parsing fails.
  /// </summary>
  public static T Deserialize<T>(string value) where T : struct, Enum
  {
    if (value is null)
      throw new ArgumentNullException(nameof(value));

    if (Enum.TryParse<T>(value, ignoreCase: true, out var result))
      return result;

    throw new ArgumentException($"Invalid enum value '{value}' for enum type '{typeof(T).Name}'", nameof(value));
  }

  /// <summary>
  /// Serialize an enum value to its string representation.
  /// </summary>
  public static string Serialize<T>(T enumValue) where T : struct, Enum
  {
    return enumValue.ToString();
  }
}

