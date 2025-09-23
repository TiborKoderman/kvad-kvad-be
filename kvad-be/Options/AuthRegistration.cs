using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public static class AuthRegistration
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection("Authentication:Schemes:Bearer"));

        // Keep claim types predictable (no legacy WS-* mappings)
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var jwt = serviceProvider.GetRequiredService<IOptions<JwtOptions>>().Value;
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,

                    ValidateAudience = true,
                    ValidAudiences = jwt.ValidAudiences,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    // Make your principal consistent:
                    NameClaimType = ClaimTypes.NameIdentifier, // we’ll put userId here
                    RoleClaimType = ClaimTypes.Role            // we’ll put roles here
                };
            });

        services.AddAuthorizationBuilder()
          .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build())
          .AddPolicy("AdminOnly", p => p.RequireRole("Admin"));

        return services;
    }
}
