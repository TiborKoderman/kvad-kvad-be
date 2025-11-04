using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    [JsonIgnore]
    public string Password { get; set; } = "";
    public string? Icon { get; set; }
    [JsonIgnore]
    public List<Role> Roles { get; set; } = [];

    [JsonIgnore]
    public Group? PrivateGroup { get; set; }
    public Guid PrivateGroupId { get; set; }

    [JsonIgnore]
    public List<Group> Groups { get; set; } = [];
    [JsonIgnore]
    public List<ChatRoom> ChatRooms { get; set; } = [];

    [Column(TypeName = "jsonb")]
    public List<kvad_be.Models.User.SidebarItem> Sidebar { get; set; } = [];
}