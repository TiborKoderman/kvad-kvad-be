public class Icon
{
  public required string Name { get; set; }
  public required IconKind Kind { get; set; }
  public required string Path { get; set; }
}

public enum IconKind
{
  Custom,
  Mdi,
}