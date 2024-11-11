
public class UserService{

    public Task<User?> getUser(string username){
        using var db = new AppDbContext();
        return Task.FromResult(db.Users.FirstOrDefault(u => u.Username == username));
    }

    public Task<User?> getUser(Guid id){
        using var db = new AppDbContext();
        return Task.FromResult(db.Users.FirstOrDefault(u => u.Id == id));
    }

    public Task<List<User>> getUsers(){
        using var db = new AppDbContext();
        return Task.FromResult(db.Users.ToList());
    }

    public Task updateUser(User user){
        using var db = new AppDbContext();
        db.Users.Update(user);
        db.SaveChanges();
        return Task.CompletedTask;
    }

    public Task deleteUser(User user){
        using var db = new AppDbContext();
        db.Users.Remove(user);
        db.SaveChanges();
        return Task.CompletedTask;
    }

    public Task<bool> uploadIcon(Guid userId, IFormFile icon){
        using var db = new AppDbContext();
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null){
            return Task.FromResult(false);
        }
        var iconPath = Path.Join("data", "user_icons", $"{userId}.png");
        using var fileStream = new FileStream(iconPath, FileMode.Create);
        icon.CopyTo(fileStream);
        user.Icon = iconPath;
        db.SaveChanges();
        return Task.FromResult(true);
    }

    public Task<bool> deleteIcon(Guid userId){
        using var db = new AppDbContext();
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null){
            return Task.FromResult(false);
        }
        if (user.Icon == null){
            return Task.FromResult(false);
        }
        File.Delete(user.Icon);
        user.Icon = null;
        db.SaveChanges();
        return Task.FromResult(true);
    }

    public Task<byte[]>? getIcon(Guid userId){
        using var db = new AppDbContext();
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null || user.Icon == null){
            return null;
        }
        var iconPath = user.Icon;
        var icon = File.ReadAllBytes(iconPath);

        return Task.FromResult(icon);

    }

}