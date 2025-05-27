using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserApp.Data;
using UserApp.Models;
using UserApp.ViewModels;  // Import du ViewModel

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

    // Chargement dynamique des sections
    public async Task<IActionResult> LoadSection(string section)
    {
        if (string.IsNullOrWhiteSpace(section))
            return BadRequest("Section invalide.");

        // Sections accessibles à tous les rôles (user, organisateur, admin)
        switch (section.ToLower())
        {
            case "profil":
                return RedirectToAction("GetProfilePartial", "Profile");

            case "reservations":
                var currentUserId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;
                if (currentUserId == null)
                    return BadRequest("Utilisateur introuvable.");

                var reservations = _context.Reservations
                                           .Where(r => r.UserId == currentUserId)
                                           .Include(r => r.Evenement)
                                           .ToList();

                return PartialView("~/Views/Home/Partials/Shared/_MesReservations.cshtml", reservations);

            case "historique":
                return PartialView("~/Views/Home/Partials/Shared/_Historique.cshtml");

            case "notifications":
                return PartialView("~/Views/Home/Partials/Shared/_Notifications.cshtml");

            case "parametres":
                return PartialView("~/Views/Home/Partials/Shared/_Parametres.cshtml");
        }

        // Sections spécifiques aux utilisateurs
        if (User.IsInRole("User"))
        {
            switch (section.ToLower())
            {
                case "paiement":
                    return PartialView("~/Views/Home/Partials/User/_Paiement.cshtml");
                case "justificatifs":
                    return PartialView("~/Views/Home/Partials/User/_Justificatifs.cshtml");
            }
        }

        // Sections spécifiques aux organisateurs
        if (User.IsInRole("Organisateur"))
        {
            switch (section.ToLower())
            {
                case "evenements":
                    var organisateurId = _userManager.GetUserId(User);
                    // Inclure la navigation Club pour accéder à Club.UserId
                    var evenements = _context.Evenements
                                             .Include(e => e.Club)
                                             .Where(e => e.Club.UserId == organisateurId)
                                             .ToList();

                    return PartialView("~/Views/Home/Partials/Organisateur/_GererEvenements.cshtml", evenements);

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

        // Section admin
        if (User.IsInRole("Admin") && section.ToLower() == "utilisateurs")
        {
            return PartialView("~/Views/Home/Partials/Admin/_GestionUtilisateurs.cshtml");
        }

        return Content("Section non autorisée ou introuvable.", "text/plain");
    }
}
