using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public ICollection<UserRole?> UserRoles { get; set; } = new List<UserRole?>();
    public ICollection<UserGroup?> UserGroups { get; set; } = new List<UserGroup?>();
}