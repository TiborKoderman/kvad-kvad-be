using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Sodium;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<string?> Authenticate(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !PasswordHash.ArgonHashStringVerify(user.Password, password))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        var key = Encoding.ASCII.GetBytes(configuration["Authentication:Schemes:Bearer:Key"] ?? "");

        var roles = await _context.UserRoles
            .Where(ur => ur.Users!.Any(u => u.Id == user.Id))
            .Select(ur => ur.Name)
            .ToListAsync();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Id.ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24*1000),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = configuration["Authentication:Schemes:Bearer:Issuer"],
            Audience = configuration["Authentication:Schemes:Bearer:Audience"]
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    // public async Task<string?> GenerateApiKey(User? user, DateTime? expirationDate)
    // {
    //     if (user == null)
    //         return null;

    //     var apiKey = Guid.NewGuid().ToString();
    //     user.ApiKey = apiKey;
    //     await _context.SaveChangesAsync();
    //     return apiKey;
    // }
    public async Task<bool> AuthenticateWithoutToken(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !PasswordHash.ArgonHashStringVerify(user.Password, password))
            return false;

        return true;
    }


    public async Task<User?> GetUser(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserById(string id)
    {
        if (!Guid.TryParse(id, out var guidId))
            return null;

        return await _context.Users.FirstOrDefaultAsync(u => u.Id == guidId);
    }

    public async Task<List<User>?> GetUsers()
    {
        return await _context.Users.Include(u => u.UserRoles).ToListAsync();
    }

    public async Task<User?> Register(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null)
            return null;

        user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Password = HashPassword(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public string HashPassword(string password)
    {
        return PasswordHash.ArgonHashString(password);
    }

    public async Task<List<string>> GetUserRoles(User user)
    {
        return await _context.UserRoles
            .Where(ur => ur.Users!.Any(u => u.Id == user.Id))
            .Select(ur => ur.Name)
            .ToListAsync();
    }


    public async Task<List<UserRole>> GetAllRoles()
    {
        return await _context.UserRoles.ToListAsync();
    }


}