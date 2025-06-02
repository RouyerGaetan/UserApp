using System;
using System.ComponentModel.DataAnnotations;

namespace UserApp.Models
{
    public class NoteEvenement
    {
        public int Id { get; set; }

        // Utilisateur ayant donné la note
        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        // Événement noté
        [Required]
        public int EvenementId { get; set; }
        public virtual Evenement Evenement { get; set; }

        [Range(1, 5, ErrorMessage = "La note doit être comprise entre 1 et 5.")]
        public int Note { get; set; }

        [StringLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères.")]
        public string? Commentaire { get; set; }

        public DateTime DateVote { get; set; }
    }
}
