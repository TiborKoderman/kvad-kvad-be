using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public class Unit{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Symbol { get; set; }
    public string Parameter { get; set; } = "";
    public required UnitType Type { get; set; } = UnitType.Base;
    public string? Dimension { get; set; } = null;
    public bool Prefixable { get; set; } = true;
    [JsonIgnore]
    public List<SIPrefix>? Prefix { get; set; } = null;
    // public JsonObject? BaseUnitRelation { get; set; } = null;
    [JsonIgnore]
    public JsonObject? BaseUnitRelation { get; set; } = null;

}

public enum UnitType
{
    Base,
    Derived
}