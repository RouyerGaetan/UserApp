using System.ComponentModel.DataAnnotations;
using UserApp.Models;

public class Club
{
    public int Id { get; set; }

    [Required]
    public string? Nom { get; set; }
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Adresse { get; set; }
    public string? Description { get; set; }

    // MODIF : UserId représente le propriétaire du club (clé étrangère vers User)
    public string UserId { get; set; }
    public virtual User User { get; set; }

    // Navigation vers les événements du club
    public ICollection<Evenement> Evenements { get; set; }

    // Navigation vers les athlètes du club
    public ICollection<Athlete> Athletes { get; set; }

    public Club()
    {
        Evenements = new HashSet<Evenement>();
        Athletes = new HashSet<Athlete>();
    }
}
