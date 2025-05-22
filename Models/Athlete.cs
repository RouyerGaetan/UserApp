public class Athlete
{
    public int Id { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public string? Poste { get; set; }
    public int NumeroMaillot { get; set; }

    // Relation vers Club (clé étrangère)
    public int ClubId { get; set; }
    public virtual Club Club { get; set; }

    // Notes données à cet athlète
    public ICollection<NoteAthlete> Notes { get; set; }
}
