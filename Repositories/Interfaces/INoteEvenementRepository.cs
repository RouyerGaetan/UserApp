using UserApp.Models;

public interface INoteEvenementRepository
{
    Task<NoteEvenement?> GetNoteByUserAndEvenementAsync(string userId, int evenementId);
    Task AddNoteAsync(NoteEvenement note);
    Task SaveChangesAsync();
}
