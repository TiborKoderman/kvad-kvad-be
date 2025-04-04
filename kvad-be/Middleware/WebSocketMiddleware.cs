using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;

    public WebSocketMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuthService authService)
    {
        if (context.Request.Path.StartsWithSegments("/ws") && context.WebSockets.IsWebSocketRequest)
        {
            string? token = context.Request.Query["token"];

            if (string.IsNullOrEmpty(token) || !ValidateToken(token, out ClaimsPrincipal principal))
            {
                context.Response.StatusCode = 401; // Unauthorized
                return;
            }

            var userId = principal.FindFirst(ClaimTypes.Name)?.Value;
            if (userId != null)
            {
                context.Items["User"] = await authService.GetUserById(userId);
            }
            else
            {
                context.Items["User"] = null;
            }

            await _next(context);
        }
        await _next(context);
    }




    public bool ValidateToken(string token, out ClaimsPrincipal principal)
    {
        var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        var key = Encoding.ASCII.GetBytes(configuration["Authentication:Schemes:Bearer:Key"] ?? "");

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = configuration["Authentication:Schemes:Bearer:Issuer"],
            ValidAudience = configuration["Authentication:Schemes:Bearer:Audience"],
            ValidateLifetime = true
        };

        try
        {
            principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return true;
        }
        catch
        {
            principal = new ClaimsPrincipal();
            return false;
        }
    }
}