public class MenuItem
{
    public required string Label { get; set; }
    public required string Route { get; set; }
    public string Icon { get; set; } = string.Empty;
    public List<MenuItem> Children { get; set; } = [];
}