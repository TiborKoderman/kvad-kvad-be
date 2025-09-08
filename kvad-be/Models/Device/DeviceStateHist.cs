using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

public class DeviceStateHist
{
  [Key, ForeignKey(nameof(Device))]
  public Guid DeviceId { get; set; }
  public required Instant Timestamp { get; set; }
}