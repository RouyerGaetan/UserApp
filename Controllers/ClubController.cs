using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserApp.Models;
using UserApp.Services;
using UserApp.ViewModels;

[Authorize(Roles = "Organisateur")]
public class ClubController : Controller
{
    private readonly IClubService _clubService;
    private readonly UserManager<User> _userManager;

    public ClubController(IClubService clubService, UserManager<User> userManager)
    {
        _clubService = clubService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var club = await _clubService.GetClubByUserIdAsync(user.Id);
        if (club == null) return NotFound();

        var model = new EditClubViewModel
        {
            ClubId = club.Id,
            Nom = club.Nom,
            Adresse = club.Adresse,
            Description = club.Description
        };

        return PartialView("~/Views/Home/Partials/Organisateur/_MonClub.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(EditClubViewModel model)
    {
        if (!ModelState.IsValid)
            return PartialView("~/Views/Home/Partials/Organisateur/_MonClub.cshtml", model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var success = await _clubService.UpdateClubAsync(model, user.Id);
        if (!success)
        {
            ModelState.AddModelError("", "Impossible de mettre à jour le club.");
            return PartialView("~/Views/Home/Partials/Organisateur/_MonClub.cshtml", model);
        }

        TempData["Message"] = "✅ Club mis à jour avec succès.";
        TempData["MessageType"] = "success";

        // Recharger les données du club mises à jour
        var updatedClub = await _clubService.GetClubByUserIdAsync(user.Id);
        var updatedModel = new EditClubViewModel
        {
            ClubId = updatedClub.Id,
            Nom = updatedClub.Nom,
            Adresse = updatedClub.Adresse,
            Description = updatedClub.Description
        };

        return PartialView("~/Views/Home/Partials/Organisateur/_MonClub.cshtml", updatedModel);
    }
}
