using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json.Nodes;
using System.Text.Json;

public class JsonObjectConverter : ValueConverter<JsonObject?, string>
{
    public JsonObjectConverter() 
        : base(
            v => ConvertToJsonString(v), // Convert to string when saving
            v => ParseJsonObject(v) // Convert back to JsonObject when loading
        ) { }
    private static string ConvertToJsonString(JsonObject? v)
    {
        return v != null ? v.ToJsonString() : "{}";
    }

    private static JsonObject? ParseJsonObject(string v)
    {
        return JsonNode.Parse(v) as JsonObject;
    }
}