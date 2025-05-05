public record TagDTO(
    string Id,
    string Name,
    string? Description,
    int? UnitId,
    string Expression,
    bool Enabled,
    bool Historicize,
    string Source
);