using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // public AppDbContext(DbContextOptions<AppDbContext> options)
    //     : base(options) { }

    public AppDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "app.db");
    }
    public string DbPath { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    public DbSet<KeyValue> KeyValues { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserGroup> UserGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>()
        .HasAlternateKey(u => u.Username);

        // modelBuilder.Entity<User>()
        // .HasMany(u => u.UserRoles!)
        // .WithMany(ur => ur.Users!)
        // .UsingEntity(j => j.ToTable("UserUserRoles"));


        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var adminUserId = Guid.Parse("cf960f59-cf1f-49cc-8b2c-de4c5e437730");

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                Id = 1,
                Name = "Admin",
            },
            new UserRole
            {
                Id = 2,
                Name = "User"
            }
        );

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                Username = "admin",
                Password = "$argon2id$v=19$m=32768,t=4,p=1$g8fJIqwvK69pwVZEFI2+NQ$X5P9Sd32U7UTUJmjFP/t6P5vW/7lNS/RQYLE3nPbvXU", // In a real application, ensure passwords are hashed
            }
        );


        //admin has admin role
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserRoles!)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.HasData(new { UsersId = adminUserId, UserRolesId = 1 }));
    }

}


