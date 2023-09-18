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
        public List<GenericModel>? Genre { get; set; }
        public List<GenericModel>? Streaming { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Year { get; set; }
        public int VoteAverage { get; set; }
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

        public CreateFilm()
        {
            Reviews = new List<Review>();
            Streaming = new List<GenericModel>();
            Genre = new List<GenericModel>();
        }
    }
}
