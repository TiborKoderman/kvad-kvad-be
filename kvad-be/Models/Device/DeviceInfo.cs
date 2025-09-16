using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using NodaTime;

public class DeviceInfo
{
  [Key, ForeignKey(nameof(Device))] public Guid DeviceId { get; set; }
  [JsonIgnore]
  public Device Device { get; set; } = null!;
  public Instant UpdatedAt { get; set; } = SystemClock.Instance.GetCurrentInstant();
  public string? Model { get; set; }
  public string? Hw { get; set; }
  public string? Fw { get; set; }
  [Column(TypeName = "jsonb")] public JsonDocument? Capabilities { get; set; }
  [Column(TypeName = "jsonb")] public JsonDocument? Settings { get; set; }
  public string? ConfigHash { get; set; }

}