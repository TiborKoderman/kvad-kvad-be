using System.Text.Json.Nodes;

public class Tag{
    public string Id { get; set; }
    public Device Device { get; set; }

    //Value can be a string, number, boolean, or object
    public JsonNode Value { get; set; }
    public Unit Unit { get; set; }

}