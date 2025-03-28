using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Apply migrations (optional)
        await db.Database.MigrateAsync();

        // Example: Seed admin user and group
        var adminUsername = "admin";

        if (!db.Users.Any(u => u.Username == adminUsername))
        {
            var adminId = Guid.Parse("cf960f59-cf1f-49cc-8b2c-de4c5e437730");

            var group = new Group
            {
                Id = adminId,
                Name = adminUsername,
            };

            var user = new User
            {
                Id = adminId,
                Username = adminUsername,
                Password = "hashedpassword",
                PrivateGroup = group,
                Groups = [group],
                Roles = [.. db.Roles.Where(r => r.Name == "Admin")],
            };

            group.Users.Add(user);

            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
    }
}
