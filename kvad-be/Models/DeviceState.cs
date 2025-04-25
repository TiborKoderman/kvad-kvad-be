using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DeviceState
{
    [Key, ForeignKey(nameof(Device))]
    public Guid DeviceId { get; set; }

    public bool Online { get; set; } = false;
    public bool Connected { get; set; } = false;
    public DateTime? LastSeen { get; set; } = null;
}