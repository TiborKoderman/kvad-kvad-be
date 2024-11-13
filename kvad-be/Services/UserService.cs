
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
        return  Task.FromResult(_context.Users.ToList());
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