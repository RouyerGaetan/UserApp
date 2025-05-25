using UserApp.ViewModels;

public interface IClubService
{
    Task<Club?> GetClubByUserIdAsync(string userId);
    Task<bool> UpdateClubAsync(EditClubViewModel model, string userId);
}
