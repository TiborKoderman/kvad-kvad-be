using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DeviceConfig
{
  [Key, ForeignKey(nameof(Device))] public Guid DeviceId { get; set; }
  [JsonIgnore]
  public Device Device { get; set; } = null!;

  public JsonDocument Config { get; set; } = null!;
  public string? ConfigHash { get; set; }
}