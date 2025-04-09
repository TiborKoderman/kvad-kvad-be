using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    StartDockerCompose();
}

var Configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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
            Array.Empty<string>()
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
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<DeviceService>();

builder.Services.AddSingleton<MqttServerService>(); // Ensures single instance
builder.Services.AddHostedService(provider => provider.GetRequiredService<MqttServerService>()); // Use the same instance
builder.Services.AddSingleton<MdnsDiscoveryService>();
builder.Services.AddHostedService(
    provider => new MdnsDiscoveryService(provider.GetRequiredService<ILogger<MdnsDiscoveryService>>())
);

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
    });


// Authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


// APP
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Development mode");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
     {
         //  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kvad API V1");
     });

    app.UseCors("AllowAll");
}

app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();


// using (var scope = app.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//     // Ensures the database file and tables are created if they don't exist
//     context.Database.EnsureCreated();

//     // Applies pending migrations, which will create the __EFMigrationsHistory table
//     context.Database.Migrate();
// }

// app.UseHttpsRedirection();
app.UseMiddleware<UserMiddleware>();
app.UseMiddleware<WebSocketMiddleware>();

app.MapControllers();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kvad API V1");
    c.RoutePrefix = "swagger";
});

await DbSeeder.SeedAsync(app.Services);


app.Run();



static void StartDockerCompose()
{
    try
    {
        Process process = new Process();
        process.StartInfo.FileName = "docker";
        process.StartInfo.Arguments = "compose up -d"; // Run docker compose in detached mode
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        Console.WriteLine("Docker Compose Output: " + output);
        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine("Docker Compose Error: " + error);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Failed to start Docker Compose: " + ex.Message);
    }
}
