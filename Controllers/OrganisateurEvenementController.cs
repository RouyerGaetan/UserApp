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
    public class OrganisateurEvenementController : Controller
    {
        private readonly IEvenementService _evenementService;
        private readonly IUserService _userService;
        private readonly IClubService _clubService; // ✅ Ajouté
        private readonly ISportService _sportService;
        private readonly ILogger<OrganisateurEvenementController> _logger;

        public OrganisateurEvenementController(
            IEvenementService evenementService,
            IUserService userService,
            IClubService clubService, // ✅ Ajouté
            ISportService sportService,
            ILogger<OrganisateurEvenementController> logger)
        {
            _evenementService = evenementService;
            _userService = userService;
            _clubService = clubService; // ✅ Assigné
            _sportService = sportService;
            _logger = logger;
        }

        private void ChargerSports()
        {
            ViewBag.Sports = _sportService.GetAllSports();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ChargerSports();
            return View();  // Views/OrganisateurEvenement/Create.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evenement evenement)
        {

            if (!ModelState.IsValid)
            {
                ChargerSports();
                return View(evenement);
            }

            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var club = await _clubService.GetClubByUserIdAsync(user.Id);
            if (club == null)
            {
                ModelState.AddModelError(string.Empty, "Vous n'avez pas de club associé.");
                ChargerSports();
                return View(evenement);
            }

            evenement.ClubId = club.Id;  // <<< Important !  
            evenement.AvailableSeats = evenement.TotalSeats;

            var result = await _evenementService.AddEvenementAsync(evenement);
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
                return View(evenement);
            }

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var evenement = await _evenementService.GetEvenementByIdWithClubAsync(id);
            if (evenement == null)
                return NotFound();

            if (evenement.Club == null || evenement.Club.UserId != user.Id)
                return Unauthorized();

            ChargerSports();
            return View(evenement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evenement updatedEvent)
        {
            if (!ModelState.IsValid)
            {
                ChargerSports();
                return View(updatedEvent);
            }

            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var originalEvent = await _evenementService.GetEvenementByIdWithClubAsync(id);
            if (originalEvent == null)
                return NotFound();

            if (originalEvent.Club == null || originalEvent.Club.UserId != user.Id)
                return Unauthorized();

            // ✅ On réaffecte le ClubId pour être sûr qu’il n’est pas modifié manuellement
            updatedEvent.ClubId = originalEvent.ClubId;

            var result = await _evenementService.UpdateEvenementAsync(updatedEvent);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors ?? new System.Collections.Generic.Dictionary<string, string>())
                    ModelState.AddModelError(error.Key, error.Value);

                ChargerSports();
                return View(updatedEvent);
            }

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var evenement = await _evenementService.GetEvenementByIdWithClubAsync(id);
            if (evenement == null) return NotFound();

            if (evenement.Club == null || evenement.Club.UserId != user.Id)
                return Unauthorized();

            return View(evenement);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var evenement = await _evenementService.GetEvenementByIdWithClubAsync(id);
            if (evenement == null) return NotFound();

            if (evenement.Club == null || evenement.Club.UserId != user.Id)
                return Unauthorized();

            var result = await _evenementService.DeleteEvenementAsync(id, user.Id);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }
    }
}
