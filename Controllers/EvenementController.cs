using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UserApp.Models;
using UserApp.Services.Interfaces;

namespace UserApp.Controllers
{
    public class EvenementController : Controller
    {
        private readonly IEvenementService _evenementService;
        private readonly ILogger<EvenementController> _logger;
        private readonly ISportService _sportService;
        private readonly IVilleService _villeService;
        public EvenementController(
            IEvenementService evenementService,
            ILogger<EvenementController> logger,
            ISportService sportService,
            IVilleService villeService)
        {
            _evenementService = evenementService;
            _logger = logger;
            _sportService = sportService;
            _villeService = villeService;
        }

        private void ChargerFiltres()
        {
            ViewBag.Sports = _sportService.GetAllSports();
            ViewBag.Villes = _villeService.GetAllVilles();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PageEvenement(
            string? searchTerm,
            string? sport,
            string? ville,
            decimal? prixMax,
            DateTime? date,
            string? filtreDate,
            int page = 1,
            int pageSize = 6,
            bool disponibleSeulement = false)
        {
            var pagedResult = await _evenementService.GetEvenementsWithFilterAsync(
                searchTerm, sport, ville, prixMax, date, filtreDate, page, pageSize, disponibleSeulement);

            int totalPages = (int)Math.Ceiling((double)pagedResult.TotalCount / pageSize);

            ViewData["searchTerm"] = searchTerm;
            ViewData["sport"] = sport;
            ViewData["ville"] = ville;
            ViewData["prixMax"] = prixMax;
            ViewData["date"] = date?.ToString("yyyy-MM-dd");
            ViewData["filtreDate"] = filtreDate;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;
            ViewData["disponibleSeulement"] = disponibleSeulement;

            ChargerFiltres();

            return View(pagedResult.Items);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            var evenement = await _evenementService.GetEvenementByIdWithClubAsync(id);

            if (evenement == null)
            {
                _logger.LogWarning("Détail de l’événement introuvable pour l’ID {Id}", id);
                return NotFound();
            }

            return View(evenement);
        }
    }
}
