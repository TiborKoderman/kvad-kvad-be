public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public ICollection<UserRole?> UserRoles { get; set; } = [];
    public ICollection<UserGroup?> UserGroups { get; set; } = [];
}