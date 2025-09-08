using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

[Index(nameof(DeviceId), nameof(Path), IsUnique = true)]
public class Tag
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int Id { get; set; } // primary key

    [ForeignKey(nameof(Device))]
    public required Guid DeviceId { get; set; }
    public required string Path { get; set; }

    public required Device Device { get; set; }
    public required TagSource Source { get; set; }
    public required bool Enabled { get; set; } = true;

    // IO direction, perspective of the device
    public IO IO { get; set; } = IO.Output;

    [ForeignKey(nameof(Unit))]
    public int? UnitId { get; set; }

    public short? Scale { get; set; } = 1;

    public required ValueKind ValueKind { get; set; }
    public ICollection<TagHistPolicy> HistPolicies { get; set; } = [];
    [NotMapped]     
    public ICollection<TagHist> History { get; set; } = [];
    public required TagCurr Curr { get; set; } = null!; // one-to-one
}



public enum ValueKind
{
    F64,
    I64,
    Enum,
    Boolean,
    String,
    Json,
    Decimal,
    Binary
}


[Flags]
public enum IO : byte
{
    None = 0,
    Output = 1 << 0, // 1
    Input = 1 << 1, // 2
    Both = Output | Input // 3
}


[Flags]
public enum TagQuality : ushort
{
    Good = 0,        // no issues
    Bad = 1 << 0,   // generic bad (fallback)
    Uncertain = 1 << 1,   // not guaranteed accurate
    Stale = 1 << 2,   // exceeded freshness/heartbeat
    Substituted = 1 << 3,   // carried-forward/interpolated/manual
    CommError = 1 << 4,   // transport/protocol/device offline
    SensorFault = 1 << 5,   // device reports sensor problem
    OutOfRange = 1 << 6,   // violates engineering limits
    Overrange = 1 << 7,   // clipped/saturated reading
    RateAbnormal = 1 << 8,   // ROC check failed
    NotCalibrated = 1 << 9,   // calibration missing/expired
    Backfilled = 1 << 10,  // data backfilled (FYI)
    Calculated = 1 << 11,  // derived tag (FYI)
    Maintenance = 1 << 12,  // optional
    // free bits for future use
}