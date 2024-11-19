using AutoMapper;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? Icon { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } =  [];
    public ICollection<UserGroup> UserGroups { get; set; } = [];

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