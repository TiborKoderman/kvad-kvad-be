using System.ComponentModel.DataAnnotations.Schema;

public class Scada
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }

    public required ScadaEntity[] Entities { get; set; }
}