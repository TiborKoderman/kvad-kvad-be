public record DashboardDTO(
    Guid? Id,
    string Name,
    String Type,
    bool Scrollable,
    string? Description,
    string? Icon,
    string? Color
    );