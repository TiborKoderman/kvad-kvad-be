public record DashboardDTO(
    Guid? Id,
    string Name,
    string? Description,
    bool Scrollable,
    string? Icon,
    string? Color
    );