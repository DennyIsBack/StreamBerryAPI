using System.ComponentModel.DataAnnotations;

namespace StreamBerryAPI.Models
{
    public class CreateFilm
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public List<Review>? Reviews { get; set; }
        [Required]
        public List<int>? GenreId { get; set; }
        [Required]
        public List<int>? StreamingId { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }

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
    }
}
