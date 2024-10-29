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
    public string DbPath { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    public DbSet<KeyValue> KeyValues { get; set; }
}


