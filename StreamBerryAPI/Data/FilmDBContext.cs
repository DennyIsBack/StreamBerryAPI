using Microsoft.EntityFrameworkCore;
using StreamBerryAPI.Models;

namespace StreamBerryAPI.Data
{
    public class FilmDBContext : DbContext
    {
        public FilmDBContext(DbContextOptions<FilmDBContext> options) : base(options)
        {

        }

        public DbSet<Film> Film { get; set; }
        public DbSet<GenericModel> GenericValues { get; set; }
        public DbSet<Review> Review { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>()
                        .HasOne<Film>()
                        .WithMany(f => f.Reviews)
                        .HasForeignKey(r => r.FilmId);

           modelBuilder.Entity<Film>()
                       .HasMany(f => f.Reviews)
                       .WithOne()
                       .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Film>()
                        .HasMany(f => f.Genre)
                        .WithOne()
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Film>()                     
                        .HasMany(f => f.Streaming)
                        .WithOne()
                        .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
