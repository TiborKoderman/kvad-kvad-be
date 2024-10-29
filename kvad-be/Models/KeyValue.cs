using System.ComponentModel.DataAnnotations;

public class KeyValue
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
}
