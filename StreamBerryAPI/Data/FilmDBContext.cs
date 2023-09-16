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



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Review>()
                        .HasOne<Film>()
                        .WithMany(f => f.Reviews)
                        .HasForeignKey(r => r.FilmId);
        }
    }
}
