using System.Text.Json.Nodes;

public class Tag{
    public required string Id { get; set; }
    public required Device Device { get; set; }

    //Value can be a string, number, boolean, or object
    public required JsonNode Value { get; set; }
    public required Unit Unit { get; set; }

}