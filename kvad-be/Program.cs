using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;


var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "KVAD API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer { token }\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

});

builder.Services.AddScoped<SystemInfoService>();
builder.Services.AddScoped<SystemServiceManagmentService>();
builder.Services.AddScoped<DockerService>();
builder.Services.AddScoped<CounterService>();
builder.Services.AddScoped<NodesService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();

// Configure the SQLite connection
builder.Services.AddDbContext<AppDbContext>();

// JWT
var jwtIssuer = builder.Configuration["Authentication:Schemes:Bearer:Issuer"];
var jwtKey = builder.Configuration["Authentication:Schemes:Bearer:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new ArgumentNullException(nameof(jwtKey), "JWT Key cannot be null or empty.");
}
var jwtAudiences = builder.Configuration.GetSection("Authentication:Schemes:Bearer:ValidAudiences").Get<string[]>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudiences = jwtAudiences,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        if (builder.Environment.IsDevelopment())
        {
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    var token = context.Request.Headers["Authorization"].ToString();
                    Console.WriteLine($"Received token: {token}");
                    return Task.CompletedTask;
                }
            };
        }

    });


// Authorization
builder.Services.AddAuthorization(options =>
{
    var defaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.DefaultPolicy = defaultPolicy;
});


// APP
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Ensures the database file and tables are created if they don't exist
    context.Database.EnsureCreated();

    // Applies pending migrations, which will create the __EFMigrationsHistory table
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAllHeaders");

}

app.UseHttpsRedirection();
// app.UseMiddleware<UserMiddleware>();

app.MapControllers();
app.MapGet("/swagger/{**slug}", async context =>
{
    context.Response.Redirect("/swagger/index.html");
}).AllowAnonymous(); // Allow anonymous access to Swagger

app.Run();
