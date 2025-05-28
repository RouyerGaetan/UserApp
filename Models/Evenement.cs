using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace UserApp.Models
{
    public class Evenement
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
        public string? Titre { get; set; }

        [Required(ErrorMessage = "La description est obligatoire.")]
        [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "La ville est obligatoire.")]
        [StringLength(100, ErrorMessage = "La ville ne peut pas dépasser 100 caractères.")]
        public string? Ville { get; set; }

        [Required(ErrorMessage = "Le sport est obligatoire.")]
        public string? Sport { get; set; }

        [Required(ErrorMessage = "La date est obligatoire.")]
        public DateTime Date { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "Le prix doit être positif et inférieur à 100 millions.")]
        [Precision(10, 2)]
        public decimal Prix { get; set; }

        [Url(ErrorMessage = "L'URL de l'image n'est pas valide.")]
        public string? ImageUrl { get; set; }

        [Required]
        public int ClubId { get; set; }
        public Club? Club { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Le nombre total de places doit être au moins 1.")]
        public int TotalSeats { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Le nombre de places disponibles ne peut pas être négatif.")]
        public int AvailableSeats { get; set; }

        public virtual ICollection<NoteEvenement> NoteEvenements { get; set; } = new List<NoteEvenement>();
    }
}
