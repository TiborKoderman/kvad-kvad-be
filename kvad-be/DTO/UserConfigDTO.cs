public record UserConfigDTO(
    string Username, 
    string Password, 
    string Icon, 
    List<int> UserRoles, 
    List<int> UserGroups
    );