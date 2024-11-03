using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddCors(options => { });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtKey = builder.Configuration["Jwt:Key"];
var validAudience = builder.Configuration.GetSection("Authentication:Schemes:Bearer:ValidAudiences").Get<string[]>();;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = false,
            ValidIssuer = jwtIssuer,
            ValidAudiences = validAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? ""))
        };
    });

// Authorization
builder.Services.AddAuthorization();


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
    app.UseCors(options => { options.AllowAnyOrigin(); options.AllowAnyMethod(); options.AllowAnyHeader(); }); app.UseCors(options => { options.AllowAnyOrigin(); options.AllowAnyMethod(); options.AllowAnyHeader(); });

}

app.UseHttpsRedirection();
// app.UseMiddleware<UserMiddleware>();

app.MapControllers();
app.Run();
