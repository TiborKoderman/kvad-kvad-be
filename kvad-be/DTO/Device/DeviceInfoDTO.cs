public sealed record DeviceInfoDTO(
    Guid Id,
    string Name,
    string? Description,
    string? Location,
    string? Model,
    string? Manufacturer,
    string? SerialNumber,
        string Fw,
    string Hw,
    string? Ip,
    int Rssi,
    float BatteryPct,
    float CpuLoadPct,
    float TempC
);