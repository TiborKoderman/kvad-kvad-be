using System.ComponentModel.DataAnnotations;

public class KeyValue
{
    [Key]
    public required string Key { get; set; }
    public required string Value { get; set; }
}
