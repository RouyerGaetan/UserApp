using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserApp.Models;
using UserApp.ViewModels;

namespace UserApp.Controllers
{
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly UserManager<User> _userManager;

        // Injection du service au lieu du context directement
        public ReservationController(IReservationService reservationService, UserManager<User> userManager)
        {
            _reservationService = reservationService;
            _userManager = userManager;
        }

        // GET: Reservation/Create/5
        public async Task<IActionResult> Create(int evenementId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.GetUserAsync(User);

            // Récupération des réservations existantes pour afficher ou rediriger
            var existingReservations = await _reservationService.GetUserReservationsAsync(user.Id);

            int totalSeatsReserved = 0;
            foreach (var res in existingReservations)
            {
                if (res.EvenementId == evenementId)
                    totalSeatsReserved += res.NumberOfSeats;
            }

            if (totalSeatsReserved >= 2)
            {
                // Déjà atteint la limite, on redirige vers la page détail de l'événement
                return RedirectToAction("Detail", "Evenement", new { id = evenementId });
            }

            var reservationViewModel = new ReservationViewModel
            {
                EvenementId = evenementId,
                NumberOfSeats = 1
            };

            return View(reservationViewModel);
        }

        // POST: Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.GetUserAsync(User);

            if (await _reservationService.TryCreateReservationAsync(model, user.Id, ModelState))
            {
                return RedirectToAction("Detail", "Evenement", new { id = model.EvenementId });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int reservationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            bool success = await _reservationService.CancelReservationAsync(reservationId, user.Id);

            return RedirectToAction("Index", "Dashboard", new { section = "reservations" });
        }
    }
}
