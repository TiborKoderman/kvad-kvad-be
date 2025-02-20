using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoMapper;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    [JsonIgnore, Required]
    public string Password { get; set; } = "";
    public string? Icon { get; set; }
    [JsonIgnore]
    public List<UserRole> UserRoles { get; set; } = [];
    [JsonIgnore]
    public List<UserGroup> UserGroups { get; set; } = [];
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
                opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Name).ToList()))
            .ForMember(
                dest => dest.UserGroups,
                opt => opt.MapFrom(src => src.UserGroups.Select(ug => ug.Name).ToList()));
        CreateMap<User, UserConfigDTO>();
    }
}