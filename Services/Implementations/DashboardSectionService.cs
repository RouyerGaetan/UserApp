using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class DashboardSectionService : IDashboardSectionService
{
    private class SectionDescriptor
    {
        public string Name { get; set; }
        public string? PartialViewPath { get; set; }
        public (string Controller, string Action)? RedirectAction { get; set; }
        public List<string> AllowedRoles { get; set; } = new List<string>();
    }

    private readonly Dictionary<string, SectionDescriptor> _sections = new Dictionary<string, SectionDescriptor>()
    {
        // Sections accessibles à tous
        ["profil"] = new SectionDescriptor
        {
            RedirectAction = ("Profile", "GetProfilePartial"),
            AllowedRoles = new List<string> { "User", "Organisateur", "Admin" }
        },
        ["reservations"] = new SectionDescriptor
        {
            RedirectAction = ("Reservation", "GetUserReservationsPartial"),
            AllowedRoles = new List<string> { "User", "Organisateur", "Admin" }
        },
        ["historique"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/Shared/_Historique.cshtml",
            AllowedRoles = new List<string> { "User", "Organisateur", "Admin" }
        },
        ["notifications"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/Shared/_Notifications.cshtml",
            AllowedRoles = new List<string> { "User", "Organisateur", "Admin" }
        },
        ["parametres"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/Shared/_Parametres.cshtml",
            AllowedRoles = new List<string> { "User", "Organisateur", "Admin" }
        },

        // Sections spécifiques User
        ["paiement"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/User/_Paiement.cshtml",
            AllowedRoles = new List<string> { "User" }
        },
        ["justificatifs"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/User/_Justificatifs.cshtml",
            AllowedRoles = new List<string> { "User" }
        },

        // Sections spécifiques Organisateur
        ["evenements"] = new SectionDescriptor
        {
            RedirectAction = ("OrganisateurEvenement", "GetOrganisateurEvenementsPartial"),
            AllowedRoles = new List<string> { "Organisateur" }
        },
        ["club"] = new SectionDescriptor
        {
            RedirectAction = ("Club", "Edit"),
            AllowedRoles = new List<string> { "Organisateur" }
        },
        ["statistiques"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/Organisateur/_Statistiques.cshtml",
            AllowedRoles = new List<string> { "Organisateur" }
        },
        ["spectateurs"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/Organisateur/_Spectateurs.cshtml",
            AllowedRoles = new List<string> { "Organisateur" }
        },
        ["places"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/Organisateur/_GestionPlaces.cshtml",
            AllowedRoles = new List<string> { "Organisateur" }
        },
        ["avis"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/Organisateur/_Avis.cshtml",
            AllowedRoles = new List<string> { "Organisateur" }
        },
        ["alertes"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/Organisateur/_Alertes.cshtml",
            AllowedRoles = new List<string> { "Organisateur" }
        },

        // Section Admin
        ["utilisateurs"] = new SectionDescriptor
        {
            PartialViewPath = "~/Views/Home/Partials/Admin/_GestionUtilisateurs.cshtml",
            AllowedRoles = new List<string> { "Admin" }
        }
    };

    public Task<IActionResult> GetSectionAsync(string section, ClaimsPrincipal user)
    {
        if (string.IsNullOrWhiteSpace(section))
            return Task.FromResult<IActionResult>(new BadRequestObjectResult("Section invalide."));

        var key = section.ToLowerInvariant();

        if (!_sections.TryGetValue(key, out var descriptor))
        {
            return Task.FromResult<IActionResult>(new ContentResult
            {
                Content = "Section non autorisée ou introuvable.",
                ContentType = "text/plain"
            });
        }

        // Vérifier les rôles de l'utilisateur
        bool isAuthorized = descriptor.AllowedRoles.Any(role => user.IsInRole(role));
        if (!isAuthorized)
        {
            return Task.FromResult<IActionResult>(new ContentResult
            {
                Content = "Section non autorisée ou introuvable.",
                ContentType = "text/plain"
            });
        }

        if (descriptor.RedirectAction.HasValue)
        {
            var (controller, action) = descriptor.RedirectAction.Value;
            return Task.FromResult<IActionResult>(new RedirectToActionResult(action, controller, null));
        }

        if (!string.IsNullOrEmpty(descriptor.PartialViewPath))
        {
            return Task.FromResult<IActionResult>(new PartialViewResult
            {
                ViewName = descriptor.PartialViewPath
            });
        }

        return Task.FromResult<IActionResult>(new ContentResult
        {
            Content = "Section non autorisée ou introuvable.",
            ContentType = "text/plain"
        });
    }
}
