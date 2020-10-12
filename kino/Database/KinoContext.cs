using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace kino.Database
{

    public class KinoContext : IdentityDbContext<User>
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ScreeningRoom> ScreeningRooms { get; set; }
        public DbSet<Screening> Screenings { get; set; }

        public KinoContext() : base()
        {
        }

        public KinoContext(DbContextOptions<KinoContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Screening>(s => {
                s.HasKey(s => s.ScreeningId);
                s.Property(s => s.ScreeningId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<ScreeningRoom>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<Movie>()
                .HasIndex(u => u.Title)
                .IsUnique();

            //modelBuilder.Entity<Screening>()
            //    .HasOne(screening => screening.ScreeningRoom)
            //    .WithMany(screeningRoom => screeningRoom.Screenings)
            //    .HasForeignKey(screeningRoom => screeningRoom.ScreeningId);

            //modelBuilder.Entity<Screening>()
            //    .HasOne<Movie>()
            //    .WithMany(m => m.Screenings)
            //    .HasForeignKey(sc => sc.ScreeningId);

            //modelBuilder.Entity<Reservation>()
            //    .HasOne<User>()
            //    .WithMany(u => u.Reservations)
            //    .HasForeignKey(r => r.UserId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Reservation>()
            //    .HasOne(reservation => reservation.Screening)
            //    .WithMany(screening => screening.Reservations)
            //    .HasForeignKey(reservation => reservation.ScreeningId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<KinoContext>
        {
            //private readonly IConfiguration configuration;

            //public DesignTimeDbContextFactory()
            //{
            //    configuration = new ConfigurationBuilder()
            //        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
            //        .AddJsonFile("appsettings.json", optional: false)
            //        .Build();
            //}

            //public DesignTimeDbContextFactory(IConfiguration configuration)
            //{
            //    this.configuration = configuration;
            //}

            public KinoContext CreateDbContext(string[] args)
            {
                var builder = new DbContextOptionsBuilder<KinoContext>();
                //builder.UseSqlServer(configuration.GetConnectionString("Development"));
                builder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=kinoDB;Trusted_Connection=True;MultipleActiveResultSets=true");
                return new KinoContext(builder.Options);
            }
        }

    }
}
