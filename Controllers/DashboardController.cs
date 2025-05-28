using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IClubService _clubService;

    public DashboardController(AppDbContext context, UserManager<User> userManager, IClubService clubService)
    {
        _context = context;
        _userManager = userManager;
        _clubService = clubService;
    }

    // Page principale du dashboard
    public IActionResult Index(string? section)
    {
        if (!string.IsNullOrEmpty(section))
        {
            ViewData["ActiveSection"] = section;
        }

        return View("~/Views/Home/Dashboard.cshtml");
    }

    // Chargement dynamique des sections en partial view ou redirection
    public async Task<IActionResult> LoadSection(string section)
    {
        if (string.IsNullOrWhiteSpace(section))
            return BadRequest("Section invalide.");

        var lowerSection = section.ToLower();

        // Accessible à tous les rôles
        switch (lowerSection)
        {
            case "profil":
                return RedirectToAction("GetProfilePartial", "Profile");

            case "reservations":
                return RedirectToAction("GetUserReservationsPartial", "Reservation");

            case "historique":
                return PartialView("~/Views/Home/Partials/Shared/_Historique.cshtml");

            case "notifications":
                return PartialView("~/Views/Home/Partials/Shared/_Notifications.cshtml");

            case "parametres":
                return PartialView("~/Views/Home/Partials/Shared/_Parametres.cshtml");
        }

        // Sections spécifiques aux Users
        if (User.IsInRole("User"))
        {
            switch (lowerSection)
            {
                case "paiement":
                    return PartialView("~/Views/Home/Partials/User/_Paiement.cshtml");

                case "justificatifs":
                    return PartialView("~/Views/Home/Partials/User/_Justificatifs.cshtml");
            }
        }

        // Sections spécifiques aux Organisateurs
        if (User.IsInRole("Organisateur"))
        {
            switch (lowerSection)
            {
                case "evenements":
                    return RedirectToAction("GetOrganisateurEvenementsPartial", "OrganisateurEvenement");

                case "club":
                    return RedirectToAction("Edit", "Club");

                case "statistiques":
                    return PartialView("~/Views/Home/Partials/Organisateur/_Statistiques.cshtml");

                case "spectateurs":
                    return PartialView("~/Views/Home/Partials/Organisateur/_Spectateurs.cshtml");

                case "places":
                    return PartialView("~/Views/Home/Partials/Organisateur/_GestionPlaces.cshtml");

                case "avis":
                    return PartialView("~/Views/Home/Partials/Organisateur/_Avis.cshtml");

                case "alertes":
                    return PartialView("~/Views/Home/Partials/Organisateur/_Alertes.cshtml");
            }
        }

        // Section Admin uniquement
        if (User.IsInRole("Admin") && lowerSection == "utilisateurs")
        {
            return PartialView("~/Views/Home/Partials/Admin/_GestionUtilisateurs.cshtml");
        }

        return Content("Section non autorisée ou introuvable.", "text/plain");
    }
}
