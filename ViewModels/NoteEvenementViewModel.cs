using System.ComponentModel.DataAnnotations;

public class NoteEvenementViewModel
{
    [Required]
    public int EvenementId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Note { get; set; }

    public string? Commentaire { get; set; }
}
