public abstract class MathConstant
{
  public required string Symbol { get; set; }
  public required Rational Value { get; set; }
  public Unit? Unit { get; set; } = null;
}