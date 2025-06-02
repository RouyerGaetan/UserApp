using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;

public class NoteEvenementRepository : INoteEvenementRepository
{
    private readonly AppDbContext _context;

    public NoteEvenementRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<NoteEvenement?> GetNoteByUserAndEvenementAsync(string userId, int evenementId)
    {
        return await _context.NotesEvenements
            .FirstOrDefaultAsync(n => n.UserId == userId && n.EvenementId == evenementId);
    }

    public async Task AddNoteAsync(NoteEvenement note)
    {
        await _context.NotesEvenements.AddAsync(note);
    }

    public async Task<List<NoteEvenement>> GetNotesByOrganisateurAsync(string organisateurId)
    {
        return await _context.NotesEvenements
            .Include(n => n.Evenement)
                .ThenInclude(e => e.Club)
            .Include(n => n.User)
            .Where(n => n.Evenement.Club.UserId == organisateurId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
