public class Athlete
{
    public int Id { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public string? Poste { get; set; }
    public int NumeroMaillot { get; set; }

    public DateTime Birthdate { get; set; }  // date de naissance
    public DateTime DateEntreeClub { get; set; } // date d'entrée dans le club

    // Relation vers Club (clé étrangère)
    public int ClubId { get; set; }
    public virtual Club Club { get; set; }

    public ICollection<NoteAthlete> Notes { get; set; }
}
