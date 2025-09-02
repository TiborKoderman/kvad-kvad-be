using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(UnitSymbol), nameof(PartSymbol))]
[Index(nameof(PartSymbol))]
public class UnitCanonicalPart
{
  [ForeignKey(nameof(Unit))]
  public required string UnitSymbol { get; set; }
  public Unit Unit { get; set; } = default!;

  // A base/canonical unit used in the definition (e.g., "kg", "m", "s")
  [ForeignKey(nameof(Part))]
  public required string PartSymbol { get; set; }
  public Unit Part { get; set; } = default!;

  [NotMapped] // Temporarily ignore for design-time compatibility
  public required Rational Exponent { get; set; } = Rational.One;
}