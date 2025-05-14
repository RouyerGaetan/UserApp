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

    public DashboardController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Page principale du dashboard
    public IActionResult Index(string? section)
    {
        ViewData["ActiveSection"] = section ?? "profil";
        return View("~/Views/Home/Dashboard.cshtml");
    }

    // Chargement dynamique des sections
    public IActionResult LoadSection(string section)
    {
        if (string.IsNullOrWhiteSpace(section))
        {
            return BadRequest("Section invalide.");
        }

        // Sections accessibles à tous les rôles (user, organisateur, admin)
        switch (section.ToLower())
        {
            case "profil":
                return PartialView("~/Views/Home/Partials/Shared/_Profil.cshtml");
            case "reservations":
                var userId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;

                if (userId == null)
                    return BadRequest("Utilisateur introuvable.");

                var reservations = _context.Reservations
                                           .Where(r => r.UserId == userId)
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
                    // Récupérer l'ID de l'utilisateur (organisateur) connecté
                    var userId = _userManager.GetUserId(User);

                    // Charger les événements associés à cet organisateur
                    var evenements = _context.Evenements
                                             .Where(e => e.UserId == userId)  // Filtrer par UserId
                                             .ToList();

                    return PartialView("~/Views/Home/Partials/Organisateur/_GererEvenements.cshtml", evenements);

                case "club":
                    return PartialView("~/Views/Home/Partials/Organisateur/_MonClub.cshtml");
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

        // Si aucune section valide
        return Content("Section non autorisée ou introuvable.", "text/plain");
    }
}
