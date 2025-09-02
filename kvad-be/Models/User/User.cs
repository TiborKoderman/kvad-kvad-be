using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using AutoMapper;

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

}