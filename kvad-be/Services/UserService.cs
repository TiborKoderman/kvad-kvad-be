using Sodium;

public class UserService{

    public Task registerUser(RegisterUserDTO user){
        var newUser = new User{
            Id = Guid.NewGuid(),
            Username = user.Username,
            Password = PasswordHash.ArgonHashString(user.Password),
        };
        using var db = new AppDbContext();
        db.Users.Add(newUser);
        db.SaveChanges();
        return Task.CompletedTask;
    }

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

    public Task<User?> authenticateUser(string username, string password){
        using var db = new AppDbContext();
        var user = db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null){
            return Task.FromResult<User?>(null);
        }
        if (PasswordHash.ArgonHashStringVerify(user.Password, password)){
            return Task.FromResult(user);
        }
        return Task.FromResult<User?>(null);
    }

}