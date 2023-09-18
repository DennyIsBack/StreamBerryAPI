using StreamBerryAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace StreamBerryAPI.Models
{
    public class GenericModel
    {
        [Key()]
        public int Id { get; set; }
        public string? Description { get; set; }
    }
}