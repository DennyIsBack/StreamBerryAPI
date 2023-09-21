namespace StreamBerryAPI.Models
{
    public class AverageByGenreYear
    {
        public int Year { get; set; }
        public int AverageRating { get; set; }
        public string? Genre { get; set; }

        public List<Film>? Data { get; set; }

        public AverageByGenreYear()
        {
            Data = new List<Film>();
        }
    }
}
