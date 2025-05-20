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
        private readonly ILogger<OrganisateurEvenementController> _logger;
        private readonly ISportService _sportService;

        public OrganisateurEvenementController(
            IEvenementService evenementService,
            IUserService userService,
            ILogger<OrganisateurEvenementController> logger,
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

            evenement.UserId = user.Id;
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

            var evenement = await _evenementService.GetEvenementByIdAsync(id);
            if (evenement == null)
                return NotFound();

            if (evenement.UserId != user.Id)
                return Unauthorized();

            ChargerSports();

            return View(evenement);  // Views/OrganisateurEvenement/Edit.cshtml
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

            var result = await _evenementService.UpdateEvenementAsync(updatedEvent);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors ?? new Dictionary<string, string>())
                    ModelState.AddModelError(error.Key, error.Value);

                ChargerSports();
                return View(updatedEvent);
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

            if (evenement.UserId != user.Id)
                return Unauthorized();

            return View(evenement);  // Views/OrganisateurEvenement/Delete.cshtml
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _evenementService.DeleteEvenementAsync(id, user.Id);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            TempData["Message"] = "L'événement a bien été supprimé.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }
    }
}
