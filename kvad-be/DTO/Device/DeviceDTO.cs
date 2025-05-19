public record DeviceDTO(
    Guid Id,
    string Name,
    string? Description,
    Guid? OwnerId,
    string? Mac,
    string? Location,
    string? Type,
    bool Virtual,
    HistorizationInterval? HistorizationInterval,
    List<TagDTO> Tags
);