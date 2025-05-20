using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;
using UserApp.Services.Interfaces;

namespace UserApp.Controllers
{
    [Authorize(Roles = "Organisateur")]
    public class EvenementController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<EvenementController> _logger;
        private readonly ISportService _sportService;

        public EvenementController(AppDbContext context, UserManager<User> userManager, ILogger<EvenementController> logger, ISportService sportService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _sportService = sportService;
        }

        // Méthode privée pour vérifier que l'utilisateur est bien l'organisateur de l'événement
        private async Task<bool> EstOrganisateurDeLEvenement(int evenementId)
        {
            User? user = await _userManager.GetUserAsync(User);
            Evenement? evenement = await _context.Evenements.FindAsync(evenementId);
            return evenement != null && evenement.UserId == user?.Id;
        }

        // GET : Formulaire de création
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Sports = _sportService.GetAllSports();
            return View();
        }

        // POST : Création
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evenement evenement)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Sports = _sportService.GetAllSports();
                return View(evenement);
            }

            User? user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            evenement.AvailableSeats = evenement.TotalSeats;

            // Associer l'événement à l'organisateur connecté
            evenement.UserId = user.Id;
            _context.Evenements.Add(evenement);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }

        // GET: Edition
        public async Task<IActionResult> Edit(int id)
        {
            Evenement? evenement = await _context.Evenements.FindAsync(id);
            User? user = await _userManager.GetUserAsync(User);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized(); // Accès interdit si ce n'est pas l'organisateur

            ViewBag.Sports = _sportService.GetAllSports();

            if(evenement.ImageUrl == "/images/default-event.jfif")
            {
                evenement.ImageUrl = null;
            }
            return View(evenement);
        }

        // POST: Edition
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evenement updatedEvent)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Sports = _sportService.GetAllSports();
                return View(updatedEvent);
            }

            Evenement? evenement = await _context.Evenements.FindAsync(id);
            User? user = await _userManager.GetUserAsync(User);

            if (evenement == null || user == null || evenement.UserId != user.Id)
                return Unauthorized();

            int reservedSeats = evenement.TotalSeats - evenement.AvailableSeats;

            evenement.Titre = updatedEvent.Titre;
            evenement.Description = updatedEvent.Description;
            evenement.Ville = updatedEvent.Ville;
            evenement.Date = updatedEvent.Date;
            evenement.Sport = updatedEvent.Sport;
            evenement.Prix = updatedEvent.Prix;
            evenement.ImageUrl = updatedEvent.ImageUrl;
            evenement.TotalSeats = updatedEvent.TotalSeats;

            evenement.AvailableSeats = CalculerPlacesDisponibles(updatedEvent.TotalSeats, reservedSeats);

            // Vérification que le nombre de places disponibles ne soit pas négatif
            if (evenement.AvailableSeats < 0)
            {
                ModelState.AddModelError("TotalSeats", "Impossible de définir moins de places que celles déjà réservées.");
                ViewBag.Sports = _sportService.GetAllSports();
                return View(updatedEvent);
            }

            _context.Update(evenement);
            await _context.SaveChangesAsync();

            TempData["Message"] = "L'événement a bien été modifié.";
            TempData["MessageType"] = "success";

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }
        private int CalculerPlacesDisponibles(int totalSeats, int reservedSeats)
        {
            return totalSeats - reservedSeats;
        }
        // GET: Suppression (confirmation)
        public async Task<IActionResult> Delete(int id)
        {
            Evenement? evenement = await _context.Evenements.FindAsync(id);
            User? user = await _userManager.GetUserAsync(User);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized();

            return View(evenement); // Confirme la suppression
        }

        // POST: Supression confirmée
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Evenement? evenement = await _context.Evenements.FindAsync(id);
            User? user = await _userManager.GetUserAsync(User);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized();

            _context.Evenements.Remove(evenement);
            await _context.SaveChangesAsync();

            TempData["Message"] = "L'événement a bien été supprimé.";
            TempData["MessageType"] = "error";

            return RedirectToAction("Index", "Dashboard", new { section = "evenements" });
        }

        // GET : Liste des évènements (accesible à tous)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> PageEvenement(string searchTerm, string sport, string ville, decimal? prixMax, DateTime? date, string filtreDate, int page = 1, int pageSize = 6)
        {
            var query = _context.Evenements.AsQueryable();

            // Filtres
            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(e => e.Titre.Contains(searchTerm));

            if (!string.IsNullOrEmpty(sport))
                query = query.Where(e => e.Sport == sport);

            if (!string.IsNullOrEmpty(ville))
                query = query.Where(e => e.Ville == ville);

            if (prixMax.HasValue)
                query = query.Where(e => e.Prix <= prixMax);

            if (date.HasValue)
                query = query.Where(e => e.Date.Date == date.Value.Date);

            if (filtreDate == "avenir")
                query = query.Where(e => e.Date > DateTime.Now);
            else if (filtreDate == "passe")
                query = query.Where(e => e.Date < DateTime.Now);

            // Pagination
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var evenements = await query
                .OrderBy(e => e.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Infos pour la vue
            ViewData["searchTerm"] = searchTerm;
            ViewData["sport"] = sport;
            ViewData["ville"] = ville;
            ViewData["prixMax"] = prixMax;
            ViewData["date"] = date;
            ViewData["filtreDate"] = filtreDate;
            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = page;

            ViewBag.Sports = _sportService.GetAllSports();

            return View(evenements);
        }


        // GET : Détail d'un évènement
        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id)
        {
            Evenement? evenement = await _context.Evenements
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evenement == null)
            {
                _logger.LogWarning("Détail de l’événement introuvable pour l’ID {Id}", id); // Logging
                return NotFound();
            }

            return View(evenement);
        }
    }
}