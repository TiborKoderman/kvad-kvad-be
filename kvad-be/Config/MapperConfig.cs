using AutoMapper;

public class MapperConfig
{
    public static Mapper InitializeAutomapper(ILoggerFactory loggerFactory)
    {
        //Provide all the Mapping Configuration
        var config = new MapperConfiguration(cfg =>
        {
            //Configuring Employee and EmployeeDTO
            cfg.CreateMap<User, UserTableDTO>()
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.Roles.Select(ur => ur.Name).ToList()))
                .ForMember(dest => dest.UserGroups, opt => opt.MapFrom(src => src.Groups.Select(ug => ug.Name).ToList()));
            //Any Other Mapping Configuration ....
        }, loggerFactory: loggerFactory);

        //Create an Instance of Mapper and return that Instance
        var mapper = new Mapper(config);
        return mapper;
    }
}