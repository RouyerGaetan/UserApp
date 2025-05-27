using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UserApp.Models;
using QRCoder;
using UserApp.ViewModels;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;

    public ReservationService(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<List<Reservation>> GetUserReservationsAsync(string userId)
    {
        return await _reservationRepository.GetReservationsByUserIdAsync(userId);
    }

    // Nouvelle méthode avec la logique pour la limite à 2 places max cumulées
    public async Task<bool> TryCreateReservationAsync(ReservationViewModel model, string userId, ModelStateDictionary modelState)
    {
        var evenement = await _reservationRepository.GetEvenementByIdAsync(model.EvenementId);
        if (evenement == null)
        {
            return false;
        }

        if (model.NumberOfSeats < 1 || model.NumberOfSeats > 2)
        {
            modelState.AddModelError("NumberOfSeats", "Vous pouvez réserver entre 1 et 2 places.");
            return false;
        }

        // Récupération du total des places déjà réservées pour cet événement par l'utilisateur
        var totalReservedSeats = await _reservationRepository.GetTotalReservedSeatsAsync(model.EvenementId, userId);
        var newTotal = totalReservedSeats + model.NumberOfSeats;

        if (newTotal > 2)
        {
            modelState.AddModelError("", $"Vous avez déjà réservé {totalReservedSeats} place(s). Vous ne pouvez pas dépasser 2 places par événement.");
            return false;
        }

        if (evenement.AvailableSeats < model.NumberOfSeats)
        {
            modelState.AddModelError("", "Il n'y a pas assez de places disponibles.");
            return false;
        }

        // Mise à jour des places disponibles
        evenement.AvailableSeats -= model.NumberOfSeats;
        await _reservationRepository.UpdateEvenementAsync(evenement);

        var reservation = new Reservation
        {
            UserId = userId,
            EvenementId = model.EvenementId,
            ReservationDate = DateTime.Now,
            NumberOfSeats = model.NumberOfSeats,
            Status = "Réservée",
            IsPresent = false,
            QRcode = GenerateQrCode(Guid.NewGuid().ToString()) // Génération QR code
        };

        await _reservationRepository.AddReservationAsync(reservation);
        await _reservationRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CancelReservationAsync(int reservationId, string userId)
    {
        var reservation = await _reservationRepository.GetReservationByIdAsync(reservationId);
        if (reservation == null || reservation.UserId != userId)
            return false;

        var evenement = await _reservationRepository.GetEvenementByIdAsync(reservation.EvenementId);
        if (evenement != null)
        {
            evenement.AvailableSeats += reservation.NumberOfSeats;
            await _reservationRepository.UpdateEvenementAsync(evenement);
        }

        await _reservationRepository.DeleteReservationAsync(reservation);
        await _reservationRepository.SaveChangesAsync();

        return true;
    }


    // Méthode privée pour générer le QR code
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
