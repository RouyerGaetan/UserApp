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

        // MODIF : Remplacement de NomDuClub par une navigation vers Club
        public virtual Club Club { get; set; }
    }
}
