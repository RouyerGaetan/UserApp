using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserApp.Models;
using UserApp.ViewModels;
using UserApp.Services;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IProfileService _profileService;

    public ProfileController(UserManager<User> userManager, IProfileService profileService)
    {
        _userManager = userManager;
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfilePartial()
    {
        var user = await _userManager.Users
                    .Include(u => u.Club)
                    .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

        if (user == null)
            return Unauthorized();

        var model = _profileService.MapUserToEditProfileViewModel(user);

        return PartialView("~/Views/Home/Partials/Shared/_EditProfile.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return PartialView("~/Views/Home/Partials/Shared/_EditProfile.cshtml", model);

        // Charge le user avec la propriété Club incluse
        var user = await _userManager.Users
                      .Include(u => u.Club)
                      .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
        if (user == null)
            return Unauthorized();

        var result = await _profileService.UpdateUserProfileAsync(user, model);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return PartialView("~/Views/Home/Partials/Shared/_EditProfile.cshtml", model);
        }

        TempData["Message"] = "Profil mis à jour avec succès.";
        TempData["MessageType"] = "success";

        return PartialView("~/Views/Home/Partials/Shared/_EditProfile.cshtml", model);
    }
}
