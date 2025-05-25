public interface IClubRepository
{
    Task<Club?> GetClubByUserIdAsync(string userId);
    Task UpdateClubAsync(Club club);

}
