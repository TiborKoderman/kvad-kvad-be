public record UserConfigDTO(
    Guid? Id,
    string Username,
    string Password,
    string Icon,
    List<int> UserRoles,
    List<int> UserGroups
    );