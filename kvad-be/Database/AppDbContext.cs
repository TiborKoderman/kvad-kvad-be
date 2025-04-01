using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        options.UseNpgsql(connectionString);
    }

    public DbSet<KeyValue> KeyValues { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Group> Groups { get; set; }

    public DbSet<Device> Devices { get; set; }

    public DbSet<Unit> Units { get; set; }
    public DbSet<SIPrefix> SIPrefixes { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagHist> TagHists { get; set; }
    public DbSet<Dashboard> Dashboards { get; set; }
    public DbSet<Layout> Layouts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<User>()
        .HasAlternateKey(u => u.Username);

        modelBuilder.Entity<Unit>()
        .Property(u => u.BaseUnitRelation)
        .HasConversion(
            new JsonObjectConverter()
        );

        modelBuilder.Entity<ChatMessage>()
            .HasKey(cm => new { cm.ChatRoomId, cm.Id });

        modelBuilder.Entity<ChatMessage>()
            .Property(cm => cm.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ChatMessage>()
            .HasOne<ChatRoom>()
            .WithMany(c => c.Messages)
            .HasForeignKey(cm => cm.ChatRoomId);



        modelBuilder.Entity<User>()
            .HasOne(u => u.PrivateGroup)
            .WithOne(g => g.PrivateOwner) // 👈 1-to-1 setup
            .HasForeignKey<User>(u => u.PrivateGroupId)
            .OnDelete(DeleteBehavior.Cascade); // optional, prevents cascade loops

        modelBuilder.Entity<TagHist>()
            .HasIndex(th => new { th.TagId, th.Timestamp })
            .IsDescending(false, true);


        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
            },
            new Role
            {
                Id = 2,
                Name = "Moderator",
            },
            new Role
            {
                Id = 3,
                Name = "User",
            }
        );

        //populate SI Prefixes
        modelBuilder.Entity<SIPrefix>().HasData(
            new SIPrefix { Id = 1, Name = "Yotta", Symbol = "Y", Factor = 1e24 },
            new SIPrefix { Id = 2, Name = "Zetta", Symbol = "Z", Factor = 1e21 },
            new SIPrefix { Id = 3, Name = "Exa", Symbol = "E", Factor = 1e18 },
            new SIPrefix { Id = 4, Name = "Peta", Symbol = "P", Factor = 1e15 },
            new SIPrefix { Id = 5, Name = "Tera", Symbol = "T", Factor = 1e12 },
            new SIPrefix { Id = 6, Name = "Giga", Symbol = "G", Factor = 1e9 },
            new SIPrefix { Id = 7, Name = "Mega", Symbol = "M", Factor = 1e6 },
            new SIPrefix { Id = 8, Name = "Kilo", Symbol = "k", Factor = 1e3 },
            new SIPrefix { Id = 9, Name = "Hecto", Symbol = "h", Factor = 1e2 },
            new SIPrefix { Id = 10, Name = "Deca", Symbol = "da", Factor = 1e1 },
            new SIPrefix { Id = 11, Name = "Deci", Symbol = "d", Factor = 1e-1 },
            new SIPrefix { Id = 12, Name = "Centi", Symbol = "c", Factor = 1e-2 },
            new SIPrefix { Id = 13, Name = "Milli", Symbol = "m", Factor = 1e-3 },
            new SIPrefix { Id = 14, Name = "Micro", Symbol = "μ", Factor = 1e-6 },
            new SIPrefix { Id = 15, Name = "Nano", Symbol = "n", Factor = 1e-9 },
            new SIPrefix { Id = 16, Name = "Pico", Symbol = "p", Factor = 1e-12 },
            new SIPrefix { Id = 17, Name = "Femto", Symbol = "f", Factor = 1e-15 },
            new SIPrefix { Id = 18, Name = "Atto", Symbol = "a", Factor = 1e-18 },
            new SIPrefix { Id = 19, Name = "Zepto", Symbol = "z", Factor = 1e-21 },
            new SIPrefix { Id = 20, Name = "Yocto", Symbol = "y", Factor = 1e-24 }
        );

        //populate Units
        modelBuilder.Entity<Unit>()
            .HasData(
            new Unit { Id = 1, Name = "Meter", Symbol = "m", Parameter = "Length", Type = UnitType.Base, Dimension = "L", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 2, Name = "Kilogram", Symbol = "kg", Parameter = "Mass", Type = UnitType.Base, Dimension = "M", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 3, Name = "Second", Symbol = "s", Parameter = "Time", Type = UnitType.Base, Dimension = "T", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 4, Name = "Ampere", Symbol = "A", Parameter = "Electric Current", Type = UnitType.Base, Dimension = "I", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 5, Name = "Kelvin", Symbol = "K", Parameter = "Temperature", Type = UnitType.Base, Dimension = "Θ", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 6, Name = "Mole", Symbol = "mol", Parameter = "Amount of Substance", Type = UnitType.Base, Dimension = "N", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 7, Name = "Candela", Symbol = "cd", Parameter = "Luminous Intensity", Type = UnitType.Base, Dimension = "J", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 8, Name = "Hertz", Symbol = "Hz", Parameter = "Frequency", Type = UnitType.Derived, Dimension = "T^-1", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 9, Name = "Newton", Symbol = "N", Parameter = "Force", Type = UnitType.Derived, Dimension = "M L T^-2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 10, Name = "Pascal", Symbol = "Pa", Parameter = "Pressure", Type = UnitType.Derived, Dimension = "M L^-1 T^-2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 11, Name = "Joule", Symbol = "J", Parameter = "Energy", Type = UnitType.Derived, Dimension = "M L^2 T^-2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 12, Name = "Watt", Symbol = "W", Parameter = "Power", Type = UnitType.Derived, Dimension = "M L^2 T^-3", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 13, Name = "Coulomb", Symbol = "C", Parameter = "Electric Charge", Type = UnitType.Derived, Dimension = "I T", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 14, Name = "Volt", Symbol = "V", Parameter = "Electric Potential", Type = UnitType.Derived, Dimension = "M L^2 T^-3 I^-1", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 15, Name = "Farad", Symbol = "F", Parameter = "Capacitance", Type = UnitType.Derived, Dimension = "M^-1 L^-2 T^4 I^2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 16, Name = "Ohm", Symbol = "Ω", Parameter = "Resistance", Type = UnitType.Derived, Dimension = "M L^2 T^-3 I^-2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 17, Name = "Siemens", Symbol = "S", Parameter = "Conductance", Type = UnitType.Derived, Dimension = "M^-1 L^-2 T^3 I^2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 18, Name = "Weber", Symbol = "Wb", Parameter = "Magnetic Flux", Type = UnitType.Derived, Dimension = "M L^2 T^-2 I^-1", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 19, Name = "Tesla", Symbol = "T", Parameter = "Magnetic Flux Density", Type = UnitType.Derived, Dimension = "M L^-1 T^-2 I^-1", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 20, Name = "Henry", Symbol = "H", Parameter = "Inductance", Type = UnitType.Derived, Dimension = "M L^2 T^-2 I^-2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 21, Name = "Lumen", Symbol = "lm", Parameter = "Luminous Flux", Type = UnitType.Derived, Dimension = "J", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 22, Name = "Lux", Symbol = "lx", Parameter = "Illuminance", Type = UnitType.Derived, Dimension = "J L^-2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 23, Name = "Becquerel", Symbol = "Bq", Parameter = "Radioactivity", Type = UnitType.Derived, Dimension = "T^-1", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 24, Name = "Gray", Symbol = "Gy", Parameter = "Absorbed Dose", Type = UnitType.Derived, Dimension = "L^2 T^-2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 25, Name = "Sievert", Symbol = "Sv", Parameter = "Equivalent Dose", Type = UnitType.Derived, Dimension = "L^2 T^-2", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 26, Name = "Katal", Symbol = "kat", Parameter = "Catalytic Activity", Type = UnitType.Derived, Dimension = "N T^-1", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 27, Name = "Apparent Power", Symbol = "VA", Parameter = "Apparent Power", Type = UnitType.Derived, Dimension = "M L^2 T^-3", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 28, Name = "Reactive Power", Symbol = "var", Parameter = "Reactive Power", Type = UnitType.Derived, Dimension = "M L^2 T^-3", Prefixable = true, BaseUnitRelation = null },
            new Unit { Id = 29, Name = "Active Power", Symbol = "W", Parameter = "Active Power", Type = UnitType.Derived, Dimension = "M L^2 T^-3", Prefixable = true, BaseUnitRelation = null }
            );
    }




}


