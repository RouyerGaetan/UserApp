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
        public User? User { get; set; }
        public decimal Prix { get; set; }
        public string? ImageUrl { get; set; }
        public string? UserId { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
    }
}
