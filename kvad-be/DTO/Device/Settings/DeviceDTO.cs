public record DeviceDTO(
    Guid Id,
    string Name,
    string? Description,
    Guid? OwnerId,
    string? Location,
    string? Type,
    bool Virtual,
    List<TagDTO> Tags,
    DeviceStateDTO? State
);