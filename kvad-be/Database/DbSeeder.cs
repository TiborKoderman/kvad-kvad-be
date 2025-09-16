using Microsoft.EntityFrameworkCore;
using kvad_be.Database;

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
                Password = "$argon2id$v=19$m=32768,t=4,p=1$g8fJIqwvK69pwVZEFI2+NQ$X5P9Sd32U7UTUJmjFP/t6P5vW/7lNS/RQYLE3nPbvXU",
                PrivateGroup = group,
                Groups = [group],
                Roles = [.. db.Roles.Where(r => r.Name == "Admin")],
                Icon = "data/user_icons/cf960f59-cf1f-49cc-8b2c-de4c5e437730.png"
            };

            group.Users.Add(user);

            db.Users.Add(user);
            await db.SaveChangesAsync();

            // Seed the virtual device and add it to the admin group
            var deviceId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var adminUserId = Guid.Parse("cf960f59-cf1f-49cc-8b2c-de4c5e437730");

            var adminGroup = db.Groups.FirstOrDefault(g => g.Id == adminUserId);
            var device = db.Devices.FirstOrDefault(d => d.Id == deviceId);

            if (adminGroup != null && device != null && !device.Groups.Contains(adminGroup))
            {
                device.Groups.Add(adminGroup);
                await db.SaveChangesAsync();
            }
        }

    }
}
