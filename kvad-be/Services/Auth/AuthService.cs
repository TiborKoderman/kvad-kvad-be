using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Sodium;
using kvad_be.Database;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly ILogger<AuthService> _logger;
    private readonly TokenService _tokens;

    public AuthService(AppDbContext db, TokenService tokens, ILogger<AuthService> logger)
    {
        _db = db;
        _logger = logger;
        _tokens = tokens;
    }
    public async Task<string?> Authenticate(string username, string password, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username, ct);
        if (user is null || !PasswordHash.ArgonHashStringVerify(user.Password, password))
            return null;

        var roles = await _db.Roles
            .Where(r => r.Users!.Any(u => u.Id == user.Id))
            .Select(r => r.Name)
            .ToListAsync(ct);

        return _tokens.CreateAccessToken(user.Id, roles);
    }



    public async Task<User?> GetUser(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUser(ClaimsPrincipal? principal)
    {
        var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null || !Guid.TryParse(userId, out var guidId))
            return null;

        return await _db.Users.FirstOrDefaultAsync(u => u.Id == guidId);
    }

    public async Task<User?> GetUserWithGroupsAndRolesById(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
            return null;

        return await _db.Users.Include(u => u.Roles)
            .Include(u => u.Groups)
            .ThenInclude(g => g.Users).FirstOrDefaultAsync(u => u.Id == guidId);
    }

    public async Task<User?> GetUserById(string? id)
    {
        if (!Guid.TryParse(id, out var guidId))
            return null;

        return await _db.Users.FirstOrDefaultAsync(u => u.Id == guidId);
    }

    public async Task<List<User>?> GetUsers()
    {
        return await _db.Users.Include(u => u.Roles).ToListAsync();
    }

    public async Task<User?> Register(string username, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null)
            return null;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Username and password cannot be empty or whitespace.");


        var userId = Guid.NewGuid();
        var privateGroup = new Group
        {
            Id = userId,
            Name = username,
            Users = []
        };

        user = new User()
        {
            Id = userId,
            Username = username,
            Password = HashPassword(password),
            PrivateGroup = privateGroup,
            Groups = [privateGroup],
        };

        privateGroup.Users.Add(user);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public string HashPassword(string password)
    {
        return PasswordHash.ArgonHashString(password);
    }

    public async Task<List<string>> GetUserRoles(User user)
    {
        return await _db.Roles
            .Where(ur => ur.Users!.Any(u => u.Id == user.Id))
            .Select(ur => ur.Name)
            .ToListAsync();
    }


    public async Task<List<Role>> GetAllRoles()
    {
        return await _db.Roles.ToListAsync();
    }


}