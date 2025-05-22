public interface IClubRepository
{
    Task<Club?> GetClubByUserIdAsync(string userId);
}
