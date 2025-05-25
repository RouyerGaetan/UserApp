using UserApp.Data;
using Microsoft.EntityFrameworkCore;


public class ClubRepository : IClubRepository
{
    private readonly AppDbContext _context;

    public ClubRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Club?> GetClubByUserIdAsync(string userId)
    {
        return await _context.Clubs
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }
    public async Task UpdateClubAsync(Club club)
    {
        _context.Clubs.Update(club);
        await _context.SaveChangesAsync();
    }

}
