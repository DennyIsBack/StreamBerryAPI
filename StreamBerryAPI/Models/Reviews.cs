namespace StreamBerryAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comments { get; set; }

        // Chave estrangeira para Film
        public int FilmId { get; set; }
    }
}
