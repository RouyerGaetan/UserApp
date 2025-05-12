using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;

namespace UserApp.Controllers
{
    [Authorize(Roles = "Organisateur")]
    public class EvenementController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<EvenementController> _logger;

        public EvenementController(AppDbContext context, UserManager<Users> userManager, ILogger<EvenementController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // Méthode privée pour centraliser les sports
        private List<string> GetSports()
        {
            return new List<string>
            {
                "Football", "Basketball", "Tennis", "Natation", "Cyclisme", "Course", "Rugby", "Handball", "Volley", "Autre"
            };
        }

        // Méthode privée pour vérifier que l'utilisateur est bien l'organisateur de l'événement
        private async Task<bool> EstOrganisateurDeLEvenement(int evenementId)
        {
            Users? user = await _userManager.GetUserAsync(User);
            Evenement? evenement = await _context.Evenements.FindAsync(evenementId);
            return evenement != null && evenement.UserId == user?.Id;
        }

        // GET : Formulaire de création
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Sports = GetSports();
            return View();
        }

        // POST : Création
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evenement evenement)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Sports = GetSports();
                return View(evenement);
            }

            Users? user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            AssignerImageParDefaut(evenement);

            // Associer l'événement à l'organisateur connecté
            evenement.UserId = user.Id;
            _context.Evenements.Add(evenement);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PageEvenement));
        }

        // GET: Edition
        public async Task<IActionResult> Edit(int id)
        {
            Evenement? evenement = await _context.Evenements.FindAsync(id);
            Users? user = await _userManager.GetUserAsync(User);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized(); // Accès interdit si ce n'est pas l'organisateur

            ViewBag.Sports = GetSports();
            return View(evenement);
        }

        // POST: Edition
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evenement updatedEvent)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Sports = GetSports();
                return View(updatedEvent);
            }

            Evenement? evenement = await _context.Evenements.FindAsync(id);
            Users? user = await _userManager.GetUserAsync(User);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized();

            evenement.Titre = updatedEvent.Titre;
            evenement.Description = updatedEvent.Description;
            evenement.Ville = updatedEvent.Ville;
            evenement.Date = updatedEvent.Date;
            evenement.Sport = updatedEvent.Sport;
            evenement.Prix = updatedEvent.Prix;
            evenement.ImageUrl = updatedEvent.ImageUrl;

            AssignerImageParDefaut(evenement);
            _context.Update(evenement);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PageEvenement));
        }
        // GET: Suppression (confirmation)
        public async Task<IActionResult> Delete(int id)
        {
            Evenement? evenement = await _context.Evenements.FindAsync(id);
            Users? user = await _userManager.GetUserAsync(User);

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
            Users? user = await _userManager.GetUserAsync(User);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized();

            _context.Evenements.Remove(evenement);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PageEvenement));
        }

        // GET : Liste des évènements (accesible à tous)
        [AllowAnonymous]
        public async Task<IActionResult> PageEvenement(string searchTerm, string sport, string ville, int? prixMax, DateTime? date, string filtreDate)
        {
            IQueryable<Evenement> evenements = _context.Evenements;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                evenements = evenements.Where(e =>
                    e.Titre.Contains(searchTerm) ||
                    e.Description.Contains(searchTerm) ||
                    e.Ville.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(sport))
            {
                evenements = evenements.Where(e => e.Sport == sport);
            }

            if (!string.IsNullOrEmpty(ville))
            {
                evenements = evenements.Where(e => e.Ville == ville);
            }

            if (prixMax.HasValue)
            {
                evenements = evenements.Where(e => e.Prix <= prixMax.Value);
            }

            if (date.HasValue)
            {
                evenements = evenements.Where(e => e.Date.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(filtreDate))
            {
                DateTime now = DateTime.Now;

                if (filtreDate == "avenir")
                {
                    evenements = evenements.Where(e => e.Date >= now);
                }
                else if (filtreDate == "passe")
                {
                    evenements = evenements.Where(e => e.Date < now);
                }
            }

            // Pour que la vue garde les valeurs des filtres
            ViewData["searchTerm"] = searchTerm;
            ViewData["sport"] = sport;
            ViewData["ville"] = ville;
            ViewData["prixMax"] = prixMax;
            ViewData["date"] = date;
            ViewData["filtreDate"] = filtreDate;

            return View(await evenements.ToListAsync());
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
        private void AssignerImageParDefaut(Evenement evenement)
        {
            if (string.IsNullOrWhiteSpace(evenement.ImageUrl))
            {
                evenement.ImageUrl = "/images/default-event.jfif";
            }
        }
    }
}
