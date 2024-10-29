using kvad_be.Controllers;
using Microsoft.EntityFrameworkCore;

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

// Configure the SQLite connection
builder.Services.AddDbContext<AppDbContext>();


var app = builder.Build();

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


app.MapControllers();
app.Run();
