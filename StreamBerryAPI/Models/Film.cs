using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace StreamBerryAPI.Models
{
    public class Film : IEntityTypeConfiguration<Film>
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<Review>? Reviews { get; set; }
        public List<GenericModel>? Genre { get; set;}
        public List<GenericModel>? Streaming { get; set; }
        public int VoteAverage { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public Film()
        {
            Reviews = new List<Review>();
            Streaming = new List<GenericModel>();
            Genre = new List<GenericModel>();
        }

        public void CalculateAverage()
        {
            int Average = 0;
            if (Reviews != null && Reviews.Any())
            {
                foreach (var item in Reviews)
                {
                    Average += item.Rating;
                }

                VoteAverage = Average / Reviews.Count();
            }
            else
                VoteAverage = 0;
        }

        public void Configure(EntityTypeBuilder<Film> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(a => a.Month)
                .IsRequired();

            builder.Property(a => a.Year)
                .IsRequired();

            builder
                .HasMany(f => f.Genre)
                .WithMany()
                .UsingEntity(j => j.ToTable("FilmGenres"));

            builder
                .HasMany(f => f.Streaming)
                .WithMany()
                .UsingEntity(j => j.ToTable("FilmStreamings"));
        }
    }
}
