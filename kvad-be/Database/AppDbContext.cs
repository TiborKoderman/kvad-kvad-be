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

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("cf960f59-cf1f-49cc-8b2c-de4c5e437730"),
                Username = "admin",
                Password = "$argon2id$v=19$m=32768,t=4,p=1$g8fJIqwvK69pwVZEFI2+NQ$X5P9Sd32U7UTUJmjFP/t6P5vW/7lNS/RQYLE3nPbvXU", // In a real application, ensure passwords are hashed
            }
        );
    }

}


