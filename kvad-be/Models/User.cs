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

    public required Group PrivateGroup { get; set; } = null!;
    public Guid PrivateGroupId { get; set; }

    [JsonIgnore]
    public List<Group> Groups { get; set; } = [];
    [JsonIgnore]
    public List<ChatRoom> ChatRooms { get; set; } = [];
}

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserTableDTO>()
            .ForMember(
                dest => dest.UserRoles,
                opt => opt.MapFrom(src => src.Roles.Select(ur => ur.Name).ToList()))
            .ForMember(
                dest => dest.UserGroups,
                opt => opt.MapFrom(src => src.Groups.Select(ug => ug.Name).ToList()));
        CreateMap<User, UserConfigDTO>();
    }
}