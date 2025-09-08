using Microsoft.EntityFrameworkCore;
using NodaTime;

[PrimaryKey(nameof(TagId), nameof(Bucket))]
public class TagHistAgg
{
  public required int TagId { get; set; }
  public required Instant Bucket { get; set; }
  public double? Min { get; set; }
  public double? Max { get; set; }
  public double? Sum { get; set; }
  public int Count { get; set; }
}