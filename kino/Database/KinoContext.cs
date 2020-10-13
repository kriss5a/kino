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
        }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<KinoContext>
        {
            public KinoContext CreateDbContext(string[] args)
            {
                var builder = new DbContextOptionsBuilder<KinoContext>();
                builder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=kinoDB;Trusted_Connection=True;MultipleActiveResultSets=true");
                return new KinoContext(builder.Options);
            }
        }

    }
}
