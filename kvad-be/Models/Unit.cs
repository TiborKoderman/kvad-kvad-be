public class Unit{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Symbol { get; set; }
    public required string Parameter { get; set; }
    public required UnitType Type { get; set; }
    public string? Dimension { get; set; }
    public bool Prefixable { get; set; } = true;
    public List<SIPrefix>? Prefix { get; set; } = null;
    public object? BaseUnitRelation { get; set; } = null;
}

public enum UnitType
{
    Base,
    Derived
}