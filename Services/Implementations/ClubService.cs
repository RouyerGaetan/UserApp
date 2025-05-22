public class ClubService : IClubService
{
    private readonly IClubRepository _clubRepository;

    public ClubService(IClubRepository clubRepository)
    {
        _clubRepository = clubRepository;
    }

    public async Task<Club?> GetClubByUserIdAsync(string userId)
    {
        return await _clubRepository.GetClubByUserIdAsync(userId);
    }
}
