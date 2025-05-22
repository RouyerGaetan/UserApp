using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Pour Include si besoin
using UserApp.Data;
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
        private readonly AppDbContext _context; // Ajout du contexte EF

        public AccountService(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context) // Injection du DbContext
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                NormalizedUserName = model.Email.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                // Suppression de NomDuClub ici, car on crée un Club séparé
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            await _userManager.AddToRoleAsync(user, model.Role);

            // ** Création du Club uniquement si l'utilisateur est Organisateur **
            if (model.Role == "Organisateur" && !string.IsNullOrEmpty(model.NomDuClub))
            {
                var club = new Club
                {
                    Nom = model.NomDuClub,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    // Tu peux ajouter d'autres propriétés selon besoin
                };

                _context.Clubs.Add(club);
                await _context.SaveChangesAsync();
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

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
