using System.ComponentModel.DataAnnotations;
using UsersApp.Attributes;

namespace UsersApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(2, MinimumLength = 1, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        public string Role { get; set; }  // Nouvelle propriété pour choisir le rôle

        [RequiredIfOrganisateur(ErrorMessage = "Le nom du club est requis pour les organisateurs.")]
        public string? NomDuClub { get; set; }  // Peut rester nullable en C#, validation fera le reste
    }
}