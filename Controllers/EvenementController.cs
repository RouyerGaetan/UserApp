using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UserApp.Models;
using UserApp.Services.Interfaces;

namespace UserApp.Controllers
{
    [Authorize(Roles = "Organisateur")]
    public class EvenementController : Controller
    {
        private readonly IEvenementService _evenementService;
        private readonly IUserService _userService;
        private readonly ILogger<EvenementController> _logger;
        private readonly ISportService _sportService;

        public EvenementController(
            IEvenementService evenementService,
            IUserService userService,
            ILogger<EvenementController> logger,
            ISportService sportService)
        {
            _evenementService = evenementService;
            _userService = userService;
            _logger = logger;
            _sportService = sportService;
        }

        private void ChargerSports()
        {
            ViewBag.Sports = _sportService.GetAllSports();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ChargerSports();
            return View("~/Views/DashboardEvenement/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evenement evenement)
        {
            if (!ModelState.IsValid)
            {
                ChargerSports();
                return View("~/Views/DashboardEvenement/Create.cshtml", evenement);
            }

            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            evenement.UserId = user.Id;
            evenement.AvailableSeats = evenement.TotalSeats;

            var result = await _evenementService.AddEvenementAsync(evenement); // <-- changement ici
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors ?? new Dictionary<string, string>())
                {
                    if (string.IsNullOrWhiteSpace(error.Key))
                        ModelState.AddModelError(string.Empty, error.Value);
                    else
                        ModelState.AddModelError(error.Key, error.Value);
                }

                ChargerSports();
                return View("~/Views/DashboardEvenement/Create.cshtml", evenement);
            }

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var evenement = await _evenementService.GetEvenementByIdAsync(id);
            if (evenement == null)
                return NotFound();

            // Vérifie si l'utilisateur est propriétaire (si besoin)
            if (evenement.UserId != user.Id)
                return Unauthorized();

            ChargerSports();

            return View("~/Views/DashboardEvenement/Edit.cshtml", evenement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evenement updatedEvent)
        {
            if (!ModelState.IsValid)
            {
                ChargerSports();
                return View("~/Views/DashboardEvenement/Edit.cshtml", updatedEvent);
            }

            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _evenementService.UpdateEvenementAsync(updatedEvent); // <-- modification ici

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors ?? new Dictionary<string, string>())
                    ModelState.AddModelError(error.Key, error.Value);

                ChargerSports();
                return View("~/Views/DashboardEvenement/Edit.cshtml", updatedEvent);
            }

            TempData["Message"] = "L'événement a bien été modifié.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var evenement = await _evenementService.GetEvenementByIdAsync(id);
            if (evenement == null) return NotFound();

            // Vérifie si l’utilisateur est propriétaire (optionnel)
            if (evenement.UserId != user.Id)
                return Unauthorized();

            return View("~/Views/DashboardEvenement/Delete.cshtml", evenement);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _evenementService.DeleteEvenementAsync(id, user.Id); // <-- modif ici
            if (!result.Succeeded)
            {
                // Tu peux aussi afficher un message d’erreur ou loguer
                return Unauthorized();
            }

            TempData["Message"] = "L'événement a bien été supprimé.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PageEvenement(string searchTerm, string sport, string ville, decimal? prixMax, DateTime? date, string filtreDate, int page = 1, int pageSize = 6)
        {
            var pagedResult = await _evenementService.GetEvenementsWithFilterAsync(searchTerm, sport, ville, prixMax, date, filtreDate, page, pageSize);

            // Calcul de TotalPages dans le contrôleur
            int totalPages = (int)Math.Ceiling((double)pagedResult.TotalCount / pageSize);

            ViewData["searchTerm"] = searchTerm;
            ViewData["sport"] = sport;
            ViewData["ville"] = ville;
            ViewData["prixMax"] = prixMax;
            ViewData["date"] = date;
            ViewData["filtreDate"] = filtreDate;
            ViewData["TotalPages"] = totalPages;    // <-- ici TotalPages calculé
            ViewData["CurrentPage"] = page;

            ChargerSports();

            return View("~/Views/Evenement/PageEvenement.cshtml", pagedResult.Items);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var evenement = await _evenementService.GetEvenementByIdAsync(id);

            if (evenement == null)
            {
                _logger.LogWarning("Détail de l’événement introuvable pour l’ID {Id}", id);
                return NotFound();
            }

            return View("~/Views/Evenement/Detail.cshtml", evenement);
        }
    }
}
