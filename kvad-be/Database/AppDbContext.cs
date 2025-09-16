using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NodaTime;
using kvad_be.Database.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace kvad_be.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Configuration is handled in Program.cs via dependency injection
        // This method is kept for potential override scenarios
    }

    public DbSet<KeyValue> KeyValues { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Group> Groups { get; set; }

    public DbSet<Device> Devices { get; set; }

    public DbSet<Unit> Units { get; set; }
    public DbSet<UnitPrefix> Prefixes { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagHist> TagHists { get; set; }
    public DbSet<Dashboard> Dashboards { get; set; }
    public DbSet<Layout> Layouts { get; set; }

    public DbSet<Widget> Widgets { get; set; }
    public DbSet<WidgetType> WidgetTypes { get; set; }
    public DbSet<DashboardType> DashboardTypes { get; set; }

    public DbSet<TagSource> TagSources { get; set; }

    public DbSet<ScadaObjectTemplate> ScadaObjectTemplates { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Use JSONB for maximum compatibility between design-time and runtime
        // This avoids issues with composite type mapping during migrations
        // Use JSONB for complex types for maximum compatibility

        // NodaTime configuration - handled automatically with UseNodaTime()
        configurationBuilder.Properties<Instant>()
            .HaveColumnType("timestamptz");

        configurationBuilder.Properties<TagQuality>()
        .HaveConversion<EnumToNumberConverter<TagQuality, ushort>>()
        .HaveColumnType("smallint");


        configurationBuilder.Properties<IO>()
            .HaveConversion<byte>()
            .HaveColumnType("smallint");

        // Configure Rational to use JSONB for now (composite type mapping requires more setup)
        configurationBuilder.Properties<Rational>()
            .HaveConversion<RationalConverter>()
            .HaveColumnType("jsonb");

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
        .HasAlternateKey(u => u.Username);

        modelBuilder.Entity<ChatMessage>()
            .HasKey(cm => new { cm.ChatRoomId, cm.Id });

        modelBuilder.Entity<Unit>(e =>
        {
            e.HasKey(x => x.Symbol);

            e.HasDiscriminator<string>("UnitKind")
             .HasValue<LinearUnit>("linear")
             .HasValue<AffineUnit>("affine")
             .HasValue<LogarithmicUnit>("log");
        });

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

        modelBuilder.Entity<TagHist>(eb =>
        {
            eb.HasKey(th => new { th.TagId, th.Ts });

            eb.ToTable(t => t.HasCheckConstraint(
            "ck_hist_max_one_value",
            @"((CASE WHEN ""V_decimal"" IS NOT NULL THEN 1 ELSE 0 END) +
                (CASE WHEN ""V_f64""    IS NOT NULL THEN 1 ELSE 0 END) +
                (CASE WHEN ""V_i64""    IS NOT NULL THEN 1 ELSE 0 END) +
                (CASE WHEN ""V_enum""   IS NOT NULL THEN 1 ELSE 0 END) +
                (CASE WHEN ""V_bool""   IS NOT NULL THEN 1 ELSE 0 END) +
                (CASE WHEN ""V_string"" IS NOT NULL THEN 1 ELSE 0 END) +
                (CASE WHEN ""V_json""   IS NOT NULL THEN 1 ELSE 0 END) +
                (CASE WHEN ""V_bytea""  IS NOT NULL THEN 1 ELSE 0 END)) <= 1"
            ));
        });


        modelBuilder.Entity<Unit>(e =>
        {
            e.HasKey(x => x.Symbol);

            e.HasDiscriminator<string>("UnitKind")
             .HasValue<LinearUnit>("linear")
             .HasValue<AffineUnit>("affine");
        });



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

        modelBuilder.Entity<DashboardType>().HasData(
            new DashboardType
            {
                Id = "custom",
                Name = "Custom",
                Description = "Custom dashboard type",
                VueComponent = "CustomDashboard.vue"
            },
            new DashboardType
            {
                Id = "masonry",
                Name = "Masonry",
                Description = "Masonry dashboard type",
                VueComponent = "MasonryDashboard.vue"
            },
            new DashboardType
            {
                Id = "grid",
                Name = "Grid",
                Description = "Grid dashboard type",
                VueComponent = "GridDashboard.vue"
            },
            new DashboardType
            {
                Id = "scada",
                Name = "Scada",
                Description = "Scada dashboard type",
                VueComponent = "ScadaDashboard.vue"
            }
        );


        modelBuilder.Entity<UnitPrefix>().HasData(
            new UnitPrefix { Name = "Quetta", Symbol = "Q", Base = 10, Exponent = 30 },
            new UnitPrefix { Name = "Ronna", Symbol = "R", Base = 10, Exponent = 27 },
            new UnitPrefix { Name = "Yotta", Symbol = "Y", Base = 10, Exponent = 24 },
            new UnitPrefix { Name = "Zetta", Symbol = "Z", Base = 10, Exponent = 21 },
            new UnitPrefix { Name = "Exa", Symbol = "E", Base = 10, Exponent = 18 },
            new UnitPrefix { Name = "Peta", Symbol = "P", Base = 10, Exponent = 15 },
            new UnitPrefix { Name = "Tera", Symbol = "T", Base = 10, Exponent = 12 },
            new UnitPrefix { Name = "Giga", Symbol = "G", Base = 10, Exponent = 9 },
            new UnitPrefix { Name = "Mega", Symbol = "M", Base = 10, Exponent = 6 },
            new UnitPrefix { Name = "Kilo", Symbol = "k", Base = 10, Exponent = 3 },
            new UnitPrefix { Name = "Hecto", Symbol = "h", Base = 10, Exponent = 2 },
            new UnitPrefix { Name = "Deca", Symbol = "da", Base = 10, Exponent = 1 },
            new UnitPrefix { Name = "Deci", Symbol = "d", Base = 10, Exponent = -1 },
            new UnitPrefix { Name = "Centi", Symbol = "c", Base = 10, Exponent = -2 },
            new UnitPrefix { Name = "Milli", Symbol = "m", Base = 10, Exponent = -3 },
            new UnitPrefix { Name = "Micro", Symbol = "μ", Base = 10, Exponent = -6 },
            new UnitPrefix { Name = "Nano", Symbol = "n", Base = 10, Exponent = -9 },
            new UnitPrefix { Name = "Pico", Symbol = "p", Base = 10, Exponent = -12 },
            new UnitPrefix { Name = "Femto", Symbol = "f", Base = 10, Exponent = -15 },
            new UnitPrefix { Name = "Atto", Symbol = "a", Base = 10, Exponent = -18 },
            new UnitPrefix { Name = "Zepto", Symbol = "z", Base = 10, Exponent = -21 },
            new UnitPrefix { Name = "Yocto", Symbol = "y", Base = 10, Exponent = -24 },
            new UnitPrefix { Name = "Ronto", Symbol = "r", Base = 10, Exponent = -27 },
            new UnitPrefix { Name = "Quecto", Symbol = "q", Base = 10, Exponent = -30 },

            //binary
            new UnitPrefix { Name = "Kibi", Symbol = "Ki", Base = 2, Exponent = 10 },
            new UnitPrefix { Name = "Mebi", Symbol = "Mi", Base = 2, Exponent = 20 },
            new UnitPrefix { Name = "Gibi", Symbol = "Gi", Base = 2, Exponent = 30 },
            new UnitPrefix { Name = "Tebi", Symbol = "Ti", Base = 2, Exponent = 40 },
            new UnitPrefix { Name = "Pebi", Symbol = "Pi", Base = 2, Exponent = 50 },
            new UnitPrefix { Name = "Exbi", Symbol = "Ei", Base = 2, Exponent = 60 },
            new UnitPrefix { Name = "Zebi", Symbol = "Zi", Base = 2, Exponent = 70 },
            new UnitPrefix { Name = "Yobi", Symbol = "Yi", Base = 2, Exponent = 80 },
            new UnitPrefix { Name = "Roni", Symbol = "Ri", Base = 2, Exponent = 90 },
            new UnitPrefix { Name = "Quin", Symbol = "Qi", Base = 2, Exponent = 100 }

        );

        // Re-enable LinearUnit seed data now that type mapping is properly configured
        modelBuilder.Entity<LinearUnit>().HasData(
            IUnitFactory.CreateUnit("m", "Meter", "Length", [1, 0, 0, 0, 0, 0, 0], null),
            IUnitFactory.CreateUnit("kg", "Kilogram", "Mass", [0, 1, 0, 0, 0, 0, 0], null),
            IUnitFactory.CreateUnit("s", "Second", "Time", [0, 0, 1, 0, 0, 0, 0], null),
            IUnitFactory.CreateUnit("A", "Ampere", "Electric Current", [0, 0, 0, 1, 0, 0, 0], null),
            IUnitFactory.CreateUnit("K", "Kelvin", "Temperature", [0, 0, 0, 0, 1, 0, 0], null),
            IUnitFactory.CreateUnit("mol", "Mole", "Amount of Substance", [0, 0, 0, 0, 0, 1, 0], null),
            IUnitFactory.CreateUnit("cd", "Candela", "Luminous Intensity", [0, 0, 0, 0, 0, 0, 1], null)
        );

        modelBuilder.Entity<Device>().HasData(
            new Device
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Virtual Device 1",
                Description = "This is a virtual device for testing purposes.",
                Location = "Lab",
                Type = "Virtual",
                Virtual = true,
                OwnerId = Guid.Parse("cf960f59-cf1f-49cc-8b2c-de4c5e437730"),
                State = new DeviceState
                {
                    DeviceId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    LastHeartbeat = null,
                    BootId = null,
                    Seq = 0,
                    UptimeSec = 0,
                    HbIntervalSec = 10,
                    HbJitterPct = 20
                },
                Info = new DeviceInfo { }
            }
        );

    }




}


