public interface IClubService
{
    Task<Club?> GetClubByUserIdAsync(string userId);
}
