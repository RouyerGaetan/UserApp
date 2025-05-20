using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UserApp.Models;
using UserApp.Services.Interfaces;

namespace UserApp.Controllers
{
    public class EvenementController : Controller
    {
        private readonly IEvenementService _evenementService;
        private readonly ISportService _sportService;

        public EvenementController(IEvenementService evenementService, ISportService sportService)
        {
            _evenementService = evenementService;
            _sportService = sportService;
        }

        // GET: Evenement/Index
        public async Task<IActionResult> PageEvenement(
            string searchTerm,
            string sport,
            string ville,
            decimal? prixMax,
            DateTime? date,
            string filtreDate,
            int page = 1)
        {
            const int pageSize = 10;

            var (evenements, totalPages) = await _evenementService.GetEvenementsFilteredAsync(
                searchTerm,
                sport,
                ville,
                prixMax,
                date,
                filtreDate,
                page,
                pageSize);

            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;

            ViewBag.Sports = _sportService.GetAllSports();

            return View("PageEvenement", evenements);
        }

        // GET: Evenement/Details/5
        public async Task<IActionResult> Detail(int id)
        {
            var evenement = await _evenementService.GetEvenementByIdAsync(id);

            if (evenement == null)
                return NotFound();

            return View("Detail", evenement);
        }

        // GET: Evenement/Create
        public IActionResult Create()
        {
            ViewBag.Sports = _sportService.GetAllSports();
            return View("~/Views/DashboardEvenement/Create.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evenement evenement)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Sports = _sportService.GetAllSports();
                return View("~/Views/DashboardEvenement/Create.cshtml", evenement);
            }

            // Récupérer l'ID de l'utilisateur connecté (AspNetUsers Id)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                // Cas où l'utilisateur n'est pas connecté : tu peux rediriger vers la connexion
                return RedirectToAction("Login", "Account");
            }

            evenement.UserId = userId;  // Assigner l'ID utilisateur à l'événement

            await _evenementService.CreateEvenementAsync(evenement);

            return RedirectToAction(nameof(PageEvenement));
        }

        // GET: Evenement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var evenement = await _evenementService.GetEvenementByIdAsync(id);
            if (evenement == null) return NotFound();

            ViewBag.Sports = _sportService.GetAllSports();
            return View("~/Views/DashboardEvenement/Edit.cshtml", evenement);
        }

        // POST: Evenement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evenement updatedEvent)
        {
            if (id != updatedEvent.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Sports = _sportService.GetAllSports(); // Recharger la liste
                return View("~/Views/DashboardEvenement/Edit.cshtml", updatedEvent);
            }

            var evenement = await _evenementService.GetEvenementByIdAsync(id);
            if (evenement == null)
                return NotFound();

            await _evenementService.UpdateEvenementAsync(evenement, updatedEvent);
            return RedirectToAction(nameof(PageEvenement));
        }

        // GET: Evenement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var evenement = await _evenementService.GetEvenementByIdAsync(id);
            if (evenement == null) return NotFound();

            return View("~/Views/DashboardEvenement/Delete.cshtml", evenement);
        }

        // POST: Evenement/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _evenementService.DeleteEvenementAsync(id);
            return RedirectToAction(nameof(PageEvenement));
        }
    }
}
