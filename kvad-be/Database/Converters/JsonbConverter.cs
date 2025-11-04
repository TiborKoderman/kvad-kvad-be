using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace kvad_be.Database.Converters;

/// <summary>
/// Deterministic value converter for JSONB columns to prevent EF Core model drift issues.
/// Uses System.Text.Json with consistent serialization options.
/// </summary>
public class JsonbConverter<T> : ValueConverter<T, string>
{
  private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
  {
    WriteIndented = false, // Ensure consistent output
    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
  };

  public JsonbConverter()
      : base(
          v => JsonSerializer.Serialize(v, Options),
          v => JsonSerializer.Deserialize<T>(v, Options)!
      )
  {
  }
}
