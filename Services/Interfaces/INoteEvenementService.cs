using UserApp.Models;

public interface INoteEvenementService
{
    Task<bool> HasUserAlreadyNotedAsync(string userId, int evenementId);
    Task SubmitNoteAsync(NoteEvenement note);
}
