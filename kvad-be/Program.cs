using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Diagnostics;
using kvad_be.Database;
using Microsoft.EntityFrameworkCore.Storage;
using NodaTime;
using kvad_be.Services.WebSocket;
using kvad_be.Filters;
using kvad_be.ModelBinders;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    StartDockerCompose();
}

var Configuration = builder.Configuration;

builder.Services.AddControllers(o => o.Filters.Add<AuthExceptionFilter>())
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    })
    .AddMvcOptions(o =>
    {
        o.ModelBinderProviders.Insert(0, new CurrentUserBinderProvider());
    });

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
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<GroupService>();
builder.Services.AddScoped<DeviceService>();
builder.Services.AddScoped<DeviceHeartbeatHandlerService>();
builder.Services.AddScoped<ScadaService>();
builder.Services.AddTransient<CurrentUserBinder>();

// Add NodaTime clock for dependency injection
builder.Services.AddSingleton<IClock>(SystemClock.Instance);

builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<TokenService>();

builder.Services.AddSingleton<TopicHub>();
builder.Services.AddSingleton<TopicActivationManager>();


// Configure global JSON serializer options
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.PropertyNameCaseInsensitive = true;
});

// Add a singleton JsonSerializerOptions for services that need it
builder.Services.AddSingleton(provider =>
{
    return new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };
});

builder.Services.AddSingleton<MqttServerService>(); // Ensures single instance
builder.Services.AddHostedService(provider => provider.GetRequiredService<MqttServerService>()); // Use the same instance

// Add device health monitoring service
builder.Services.AddHostedService<DeviceHealthMonitorService>();
// builder.Services.AddSingleton<MdnsDiscoveryService>();
// builder.Services.AddHostedService(
//     provider => new MdnsDiscoveryService(provider.GetRequiredService<ILogger<MdnsDiscoveryService>>(), )
// );
builder.Services.AddSingleton<MdnsService>();
builder.Services.AddHostedService<MdnsDiscoveryService>();


builder.Services.AddSingleton<IRelationalTypeMappingSourcePlugin, RationalTypeMappingPlugin>();


// Configure the PostgreSQL connection with single source of truth
builder.Services.AddAppDbContext(builder.Configuration);
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// APP
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync().ConfigureAwait(false);   // auto-applies CreatePgTypes + others
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
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


// 3) Map the WebSocket endpoint -> TopicHub.ConnectClientAsync
// app.Map("/ws", async (HttpContext ctx, TopicHub hub) => await hub.ConnectClientAsync(ctx));


app.MapControllers();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kvad API V1");
    c.RoutePrefix = "swagger";
});

await DbSeeder.SeedAsync(app.Services).ConfigureAwait(false);


await app.RunAsync().ConfigureAwait(false);



static void StartDockerCompose()
{
    try
    {
        using Process process = new();
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
    catch (InvalidOperationException ex)
    {
        Console.WriteLine("Failed to start Docker Compose: " + ex.Message);
        throw;
    }
    catch (System.ComponentModel.Win32Exception ex)
    {
        Console.WriteLine("Failed to start Docker Compose: " + ex.Message);
        throw;
    }
}
