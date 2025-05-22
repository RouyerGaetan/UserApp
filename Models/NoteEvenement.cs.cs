using UserApp.Models;

public class NoteEvenement
{
    public int Id { get; set; }

    // Utilisateur ayant donné la note
    public string UserId { get; set; }
    public virtual User User { get; set; }

    // Événement noté
    public int EvenementId { get; set; }
    public virtual Evenement Evenement { get; set; }

    public int Note { get; set; }
    public string? Commentaire { get; set; }
    public DateTime DateVote { get; set; }
}
