using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

  public string CreateAccessToken(Guid userId, IEnumerable<string> roles, IEnumerable<Claim>? extraClaims = null)
  {
    var now = DateTime.UtcNow;

    var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
        };
    claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

    if (extraClaims is not null)
      claims.AddRange(extraClaims);

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

}
