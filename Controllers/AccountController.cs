using Microsoft.AspNetCore.Mvc;
using UserApp.Models;
using UserApp.Services.Interfaces;  // <- Changement : utilisation du service via interface
using UsersApp.ViewModels;

namespace UserApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;  // <- Changement : injection d'IAccountService au lieu de UserManager, SignInManager, RoleManager

        public AccountController(IAccountService accountService)  // <- Changement : constructeur simplifié avec seulement le service
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // <- Changement : on délègue la connexion au service, plus de gestion directe des managers ici
            bool success = await _accountService.LoginUserAsync(model);

            if (success)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, "Email or password is incorrect");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // <- Changement : utilisation du service pour créer l'utilisateur et gérer les rôles
            (bool Succeeded, IEnumerable<string> Errors) result = await _accountService.RegisterUserAsync(model);

            if (result.Succeeded)
                return RedirectToAction("Login", "Account");

            foreach (string error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        [HttpGet]
        public IActionResult VerifyEmail() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // <- Changement : on récupère l'utilisateur via le service, pas directement via UserManager
            User? user = await _accountService.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Something is wrong!");
                return View(model);
            }

            return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
        }

        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("VerifyEmail", "Account");

            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something is wrong!");
                return View(model);
            }

            // <- Changement : on délègue le changement de mot de passe au service
            bool success = await _accountService.ChangePasswordAsync(model);

            if (success)
                return RedirectToAction("Login", "Account");

            ModelState.AddModelError("", "User not found or error changing password.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // <- Changement : on délègue la déconnexion au service
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
