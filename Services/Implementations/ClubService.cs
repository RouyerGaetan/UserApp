using UserApp.ViewModels;

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
    public async Task<bool> UpdateClubAsync(EditClubViewModel model, string userId)
    {
        var existingClub = await _clubRepository.GetClubByUserIdAsync(userId);
        if (existingClub == null) return false;

        existingClub.Nom = model.Nom;
        existingClub.Adresse = model.Adresse;
        existingClub.Description = model.Description;

        await _clubRepository.UpdateClubAsync(existingClub);
        return true;
    }
}
