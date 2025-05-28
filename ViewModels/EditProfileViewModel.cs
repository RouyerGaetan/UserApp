using System;
using System.ComponentModel.DataAnnotations;

namespace UserApp.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string? Nom { get; set; }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le prénom ne peut pas dépasser 100 caractères.")]
        public string? Prenom { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date de naissance")]
        public DateTime? Birthdate { get; set; }

        [Url(ErrorMessage = "L'URL de l'avatar n'est pas valide.")]
        [Display(Name = "URL de l'avatar")]
        public string? AvatarURL { get; set; }

        [Phone(ErrorMessage = "Le numéro de téléphone n'est pas valide.")]
        [Display(Name = "Téléphone")]
        public string? PhoneNumber { get; set; }
    }
}
