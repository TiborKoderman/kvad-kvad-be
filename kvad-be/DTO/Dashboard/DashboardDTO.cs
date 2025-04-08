public record DashboardDTO(
    Guid? Id,
    string Name,
    string? Description,
    bool Scrollable,
    string? Type,
    string? Icon,
    string? Color
    );