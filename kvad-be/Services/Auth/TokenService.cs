using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public sealed class TokenService
{
  private readonly JwtOptions _opt;
  private readonly SigningCredentials _creds;

  public TokenService(IOptions<JwtOptions> options)
  {
    _opt = options.Value;
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
    _creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
  }

  public string CreateAccessToken(Guid userId, IEnumerable<string> roles)
  {
    var now = DateTime.UtcNow;

    var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };
    claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

    var token = new JwtSecurityToken(
        issuer: _opt.Issuer,
        audience: _opt.Audience, // pick one or issue per-audience if you want
        claims: claims,
        notBefore: now,
        expires: now.AddMinutes(_opt.LifetimeMinutes),
        signingCredentials: _creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
    
    public string VerifyToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_opt.Key);
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _opt.Issuer,
            ValidAudience = _opt.Audience,
            ValidateLifetime = true
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        return userId;
    }
}
