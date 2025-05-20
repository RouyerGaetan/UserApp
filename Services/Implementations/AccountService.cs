using Microsoft.AspNetCore.Identity;
using UserApp.Models;
using UserApp.Services.Interfaces;
using UsersApp.ViewModels;

namespace UserApp.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                NormalizedUserName = model.Email.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                NomDuClub = model.Role == "Organisateur" ? model.NomDuClub : null
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            await _userManager.AddToRoleAsync(user, model.Role);
            await _signInManager.SignInAsync(user, false);

            return (true, Enumerable.Empty<string>());
        }

        public async Task<bool> LoginUserAsync(LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            return result.Succeeded;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByNameAsync(email);
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null) return false;

            var result = await _userManager.RemovePasswordAsync(user);
            if (!result.Succeeded) return false;

            result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
