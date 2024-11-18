using AutoMapper;

public class MapperConfig
    {
        public static Mapper InitializeAutomapper()
        {
            //Provide all the Mapping Configuration
            var config = new MapperConfiguration(cfg =>
            {
                    //Configuring Employee and EmployeeDTO
                    cfg.CreateMap<User, UserTableDTO>()
                    .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Name).ToList()))
                    .ForMember(dest => dest.UserGroups, opt => opt.MapFrom(src => src.UserGroups.Select(ug => ug.Name).ToList()));
                    //Any Other Mapping Configuration ....
                });

            //Create an Instance of Mapper and return that Instance
            var mapper = new Mapper(config);
            return mapper;
        }
    }