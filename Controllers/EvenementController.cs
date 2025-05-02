using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserApp.Controllers
{
    [Authorize(Roles = "Organisateur")]
    public class EvenementController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;

        public EvenementController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Affiche le formulaire
        public IActionResult CreateEvenement()
        {
            ViewBag.Sports = new List<string>
            {
                "Football",
                "Basketball",
                "Tennis",
                "Natation",
                "Cyclisme",
                "Course",
                "Rugby",
                "Handball",
                "Volley",
                "Autre"
            };

            return View();
        }

        // Traite le formulaire
        [HttpPost]
        public async Task<IActionResult> Create(Evenement evenement)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Associer l'événement à l'organisateur connecté
            evenement.UserId = user.Id;

            _context.Evenements.Add(evenement);
            await _context.SaveChangesAsync();

            return RedirectToAction("PageEvenement", "Evenement");
        }
        // GET: /Evenement/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var evenement = await _context.Evenements.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized(); // Bloque l'accès si ce n'est pas son événement

            ViewBag.Sports = new List<string>
            {
                "Football",
                "Basketball",
                "Tennis",
                "Natation",
                "Cyclisme",
                "Course",
                "Rugby",
                "Handball",
                "Volley",
                "Autre"
            };

            return View(evenement);
        }

        // POST: /Evenement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Evenement updatedEvent)
        {
            var user = await _userManager.GetUserAsync(User);
            var evenement = await _context.Evenements.FindAsync(id);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized();

            evenement.Titre = updatedEvent.Titre;
            evenement.Description = updatedEvent.Description;
            evenement.Ville = updatedEvent.Ville;
            evenement.Date = updatedEvent.Date;
            evenement.Sport = updatedEvent.Sport;
            evenement.Date = updatedEvent.Date;
            evenement.Prix = updatedEvent.Prix;
            evenement.ImageUrl = updatedEvent.ImageUrl;

            _context.Update(evenement);
            await _context.SaveChangesAsync();

            return RedirectToAction("PageEvenement");
        }
        // GET: /Evenement/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var evenement = await _context.Evenements.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized();

            return View(evenement); // Confirme la suppression
        }

        // POST: /Evenement/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evenement = await _context.Evenements.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (evenement == null || evenement.UserId != user.Id)
                return Unauthorized();

            _context.Evenements.Remove(evenement);
            await _context.SaveChangesAsync();

            return RedirectToAction("PageEvenement");
        }

        [AllowAnonymous]
        public async Task<IActionResult> PageEvenement(string searchTerm, string sport, string ville, int? prixMax, DateTime? date, string filtreDate)
        {
            var evenements = _context.Evenements.AsQueryable();

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
                var now = DateTime.Now;

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
        public async Task<IActionResult> DetailEvenement(int id)
        {
            var evenement = await _context.Evenements
                .Include(e => e.User) // pour l'organisateur
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evenement == null)
            {
                return NotFound();
            }

            return View(evenement);
        }

        [AllowAnonymous]
        public IActionResult VueApiEvenements()
        {
            return View();
        }
    }
}
