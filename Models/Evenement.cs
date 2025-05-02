using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserApp.Models
{
    public class Evenement
    {
        public int Id { get; set; }
        public string? Titre { get; set; }
        public string? Description { get; set; }
        public string? Ville { get; set; }
        public string? Sport { get; set; }
        public DateTime Date { get; set; }
        [Precision(10, 2)] // 👈 max 99999999.99
        public Users? User { get; set; }
        public decimal Prix { get; set; }
        public string ImageUrl { get; set; } = "default-image.jpg";
        public string? UserId { get; set; }
    }
}
