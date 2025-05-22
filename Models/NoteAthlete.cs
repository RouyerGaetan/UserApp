using UserApp.Models;

public class NoteAthlete
{
    public int Id { get; set; }

    // L'utilisateur qui a donné la note
    public string UserId { get; set; }
    public virtual User User { get; set; }

    // Événement lié à la note
    public int EvenementId { get; set; }
    public virtual Evenement Evenement { get; set; }

    // Athlète noté
    public int AthleteId { get; set; }
    public virtual Athlete Athlete { get; set; }

    public int Note { get; set; }
    public string? Commentaire { get; set; }
    public DateTime DateVote { get; set; }
}
