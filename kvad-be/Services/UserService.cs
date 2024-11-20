
using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class UserService{

    private readonly AppDbContext _context;


    public UserService(AppDbContext context){
        _context = context;
    }

    public Task<User?> getUser(Guid id){
        return Task.FromResult(_context.Users.FirstOrDefault(u => u.Id == id));
    }

    public Task<User?> getUser(string username){
        return Task.FromResult(_context.Users.FirstOrDefault(u => u.Username == username));
    }

    public  Task<List<User>> getUsers(){
        return  Task.FromResult(_context.Users.Include(u => u.UserRoles).ToList());
    }

    public Task<List<UserTableDTO>> getUserTable(){
        var users = _context.Users.Include(u => u.UserRoles).Include(u => u.UserGroups).ToList();
        var userTable = new List<UserTableDTO>();
        foreach (var user in users){
            var userRoles = user.UserRoles.Select(ur => ur.Name).ToList();
            var userGroups = user.UserGroups.Select(ug => ug.Name).ToList();
            userTable.Add(new UserTableDTO(user.Id, user.Username, userRoles, userGroups));
        }
        return Task.FromResult(userTable);
    }

    // public Task<List<UserTableDTO> getUserTable(){
    //     var users = _context.Users.ToList();
    //     var userTable = new List<UserTableDTO>();
    //     foreach (var user in users){l
    //         var userRoles = _context.UserRoles.Where(ur => ur.UserId == user.Id).Select(ur => ur.Role.Name).ToList();
    //         var userGroups = _context.UserGroups.Where(ug => ug.UserId == user.Id).Select(ug => ug.Group.Name).ToList();
    //         userTable.Add(new UserTableDTO(user.Id, user.Username, userRoles, userGroups));
    //     }
    //     return Task.FromResult(userTable);
    // }

    public Task updateUser(User user){
        _context.Users.Update(user);
        _context.SaveChanges();
        return Task.CompletedTask;
    }

    public Task deleteUser(User user){
        _context.Users.Remove(user);
        _context.SaveChanges();
        return Task.CompletedTask;
    }
    // public Task editUser(UserConfigDTO userConfig){
    //     var user = _context.Users.FirstOrDefault(u => u.Id == userConfig.Id);
    //     if (user == null){
    //         user = new User { Id = Guid.NewGuid(), Username = userConfig.Username, Password = userConfig.Password };
    //         _context.Users.Add(user);
    //     }
    //     user.Username = userConfig.Username;
    //     user.Password = userConfig.Password;
    //     user.Icon = userConfig.Icon;
    //     user.UserRoles = userConfig.UserRoles.Select(ur => new UserRole{Id = user.Id, RoleId = ur, Name = _context.Roles.FirstOrDefault(r => r.Id == ur)?.Name}).ToList();
    //     user.UserGroups = userConfig.UserGroups.Select(ug => new UserGroup{Id = user.Id, GroupId = ug, Name = _context.Groups.FirstOrDefault(g => g.Id == ug)?.Name}).ToList();
    //     _context.SaveChanges();
    //     return Task.CompletedTask;
    // }

    public Task<UserConfigDTO?> getUserConfig(Guid userId){
        var configuration = new MapperConfiguration(cfg => cfg.CreateMap<User, UserConfigDTO>());
        var mapper = configuration.CreateMapper();

        var user = _context.Users.Include(u => u.UserRoles).Include(u => u.UserGroups).FirstOrDefault(u => u.Id == userId);
        if (user == null){
            return Task.FromResult<UserConfigDTO?>(null);
        }
        var userConfig = mapper.Map<UserConfigDTO>(user);

        return Task.FromResult(userConfig);
    }

    public Task<bool> uploadIcon(Guid userId, IFormFile icon){
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null){
            return Task.FromResult(false);
        }
        var iconPath = Path.Join("data", "user_icons", $"{userId}.png");
        using var fileStream = new FileStream(iconPath, FileMode.Create);
        icon.CopyTo(fileStream);
        user.Icon = iconPath;
        _context.SaveChanges();
        return Task.FromResult(true);
    }

    public Task<bool> deleteIcon(Guid userId){
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null){
            return Task.FromResult(false);
        }
        if (user.Icon == null){
            return Task.FromResult(false);
        }
        File.Delete(user.Icon);
        user.Icon = null;
        _context.SaveChanges();
        return Task.FromResult(true);
    }

    public Task<byte[]>? getIcon(Guid userId){
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null || user.Icon == null){
            return null;
        }
        var iconPath = user.Icon;
        var icon = File.ReadAllBytes(iconPath);

        return Task.FromResult(icon);

    }

}