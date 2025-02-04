using System.ComponentModel.DataAnnotations;

public class SIPrefix {
    [Key]
    public required int Id { get; set; }
    public required string  Symbol { get; set; }
    public required string  Name { get; set; }
    public required double Factor { get; set; }
}