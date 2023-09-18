namespace StreamBerryAPI.Models
{
    public class FilmVoteAverageByGenre
    {
        public int TotalVoteAverage { get; set; }

        public string Genre { get; set; }

        public int Year { get; set; }

        public List<Film>? Data { get; set; }

        public FilmVoteAverageByGenre()
        {
            Data = new List<Film>();
        }

    }
}
