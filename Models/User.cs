using Microsoft.AspNetCore.Identity;

namespace UserApp.Models
{
    public class User : IdentityUser
    {
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public string? AvatarURL { get; set; }
        public string? NomDuClub { get; set; }
    }
}
