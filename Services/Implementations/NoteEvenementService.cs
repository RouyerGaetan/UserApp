using UserApp.Models;

public class NoteEvenementService : INoteEvenementService
{
    private readonly INoteEvenementRepository _noteRepo;

    public NoteEvenementService(INoteEvenementRepository noteRepo)
    {
        _noteRepo = noteRepo;
    }

    public async Task<bool> HasUserAlreadyNotedAsync(string userId, int evenementId)
    {
        var existing = await _noteRepo.GetNoteByUserAndEvenementAsync(userId, evenementId);
        return existing != null;
    }

    public async Task SubmitNoteAsync(NoteEvenement note)
    {
        await _noteRepo.AddNoteAsync(note);
        await _noteRepo.SaveChangesAsync();
    }
}
