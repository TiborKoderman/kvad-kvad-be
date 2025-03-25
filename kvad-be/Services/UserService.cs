
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
        return  Task.FromResult(_context.Users.Include(u => u.Roles).ToList());
    }

    public Task<List<UserTableDTO>> getUserTable(){
        var users = _context.Users.Include(u => u.Roles).Include(u => u.Groups).ToList();
        var userTable = new List<UserTableDTO>();
        foreach (var user in users){
            var roles = user.Roles.Select(ur => ur.Name).ToList();
            var groups = user.Groups.Select(ug => ug.Name).ToList();
            userTable.Add(new UserTableDTO(user.Id, user.Username, roles, groups));
        }
        return Task.FromResult(userTable);
    }

    public Task addUser(User user){
        _context.Users.Add(user);
        _context.SaveChanges();
        return Task.CompletedTask;
    }

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

    public Task<UserConfigDTO?> getUserConfig(Guid userId){
        var configuration = new MapperConfiguration(cfg => cfg.CreateMap<User, UserConfigDTO>());
        var mapper = configuration.CreateMapper();

        var user = _context.Users.Include(u => u.Roles).Include(u => u.Groups).FirstOrDefault(u => u.Id == userId);
        if (user == null){
            return Task.FromResult<UserConfigDTO?>(null);
        }
        var userConfig = mapper.Map<UserConfigDTO?>(user);

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

    public Task<byte[]?> getIcon(Guid? userId){
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user?.Icon == null){
            return Task.FromResult<byte[]?>(null); 
        }
        var iconPath = user.Icon;
        var icon = File.ReadAllBytes(iconPath);

        return Task.FromResult<byte[]?>(icon);
    }

}