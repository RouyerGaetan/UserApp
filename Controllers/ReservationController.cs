using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;
using UserApp.ViewModels;
using QRCoder;

namespace UserApp.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReservationController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservation/Create/5
        public async Task<IActionResult> Create(int evenementId)
        {
            var evenement = await _context.Evenements
                .FirstOrDefaultAsync(e => e.Id == evenementId);

            if (evenement == null)
            {
                return NotFound();
            }

            // Récupérer l'utilisateur actuel
            var user = await _userManager.GetUserAsync(User);

            // Vérifier si l'utilisateur a déjà réservé pour cet événement
            var existingReservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.EvenementId == evenementId && r.UserId == user.Id);

            if (existingReservation != null)
            {
                return RedirectToAction("Detail", "Evenement", new { id = evenementId });
            }

            var reservationViewModel = new ReservationViewModel
            {
                EvenementId = evenement.Id,
                NumberOfSeats = 1 // Par défaut, on réserve 1 place
            };

            return View(reservationViewModel);
        }

        // POST: Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.GetUserAsync(User);
                Evenement? evenement = await _context.Evenements
                    .FirstOrDefaultAsync(e => e.Id == model.EvenementId);

                if (evenement == null)
                {
                    return NotFound();
                }

                if (model.NumberOfSeats < 1 || model.NumberOfSeats > 2)
                {
                    ModelState.AddModelError("NumberOfSeats", "Vous pouvez réserver entre 1 et 2 places.");
                    return View(model);
                }

                if (evenement.AvailableSeats < model.NumberOfSeats)
                {
                    ModelState.AddModelError("", "Il n'y a pas assez de places disponibles.");
                    return View(model);
                }

                evenement.AvailableSeats -= model.NumberOfSeats;
                _context.Evenements.Update(evenement);

                Reservation? reservation = new Reservation
                {
                    UserId = user.Id,
                    EvenementId = evenement.Id,
                    ReservationDate = DateTime.Now,
                    NumberOfSeats = model.NumberOfSeats,
                    Status = "Réservée",
                    IsPresent = false,
                    QRcode = GenerateQrCode(Guid.NewGuid().ToString()) // Génération d'un QRcode
                };

                _context.Add(reservation);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Réservation effectuée avec succès !";
                return RedirectToAction("Detail", "Evenement", new { id = evenement.Id });
            }

            return View(model);
        }

        private string GenerateQrCode(string content)
        {
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);
                return Convert.ToBase64String(qrCodeBytes);
            }
        }
    }

}
