using UserApp.Models;
using UsersApp.ViewModels;

namespace UserApp.Services.Interfaces
{
    public interface IAccountService
    {
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterUserAsync(RegisterViewModel model);
        Task<bool> LoginUserAsync(LoginViewModel model);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> ChangePasswordAsync(ChangePasswordViewModel model);
        Task LogoutAsync();
    }
}
