namespace StreamBerryAPI.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<Review>? Reviews { get; set; }
        public List<int>? GenreId { get; set;}
        public List<int>? StreamingId { get; set; }

        public int VoteAverage { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public Film()
        {
            Reviews = new List<Review>();
            StreamingId = new List<int>();
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
    }
}
