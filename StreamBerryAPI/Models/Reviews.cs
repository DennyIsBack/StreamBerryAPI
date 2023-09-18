using System.ComponentModel.DataAnnotations;

namespace StreamBerryAPI.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comments { get; set; }

        // Chave estrangeira para Film
        public int FilmId { get; set; }
    }
}
