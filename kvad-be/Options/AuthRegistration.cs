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

            NameClaimTypeRetriever = (token, claimType) => ClaimTypes.NameIdentifier,

            // Make your principal consistent:
            NameClaimType = ClaimTypes.NameIdentifier, // we’ll put userId here
            RoleClaimType = ClaimTypes.Role            // we’ll put roles here
          };

          options.Events = new JwtBearerEvents
          {
            OnMessageReceived = ctx =>
            {
              if (ctx.Request.Path.StartsWithSegments("/ws") &&
                  ctx.Request.Headers.Connection == "Upgrade" &&
                  ctx.Request.Headers.Upgrade == "websocket")
              {
                var token = ctx.Request.Query["token"];
                if (!string.IsNullOrEmpty(token))
                {
                  ctx.Token = token;
                }
                else if (ctx.Request.Headers.ContainsKey("Authorization"))
                {
                  var authHeader = ctx.Request.Headers.Authorization.ToString();
                  if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                  {
                    ctx.Token = authHeader["Bearer ".Length..].Trim();
                  }
                }
                else if (ctx.Request.Headers.SecWebSocketProtocol.Count > 0)
                {
                  var proto = ctx.Request.Headers.SecWebSocketProtocol;
                  if (proto.Count >= 2 && string.Equals(proto[0], "bearer", StringComparison.OrdinalIgnoreCase))
                    ctx.Token = proto[1];
                }
              }
              return Task.CompletedTask;
            }
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
