using System.Collections.Generic;
using System.Threading.Tasks;
using UserApp.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UserApp.ViewModels;

public interface IReservationService
{
    Task<List<Reservation>> GetUserReservationsAsync(string userId);

    // Nouvelle méthode pour essayer de créer une réservation avec la logique de limite par utilisateur
    Task<bool> TryCreateReservationAsync(ReservationViewModel model, string userId, ModelStateDictionary modelState);

    Task<bool> CancelReservationAsync(int reservationId, string userId);

    Task<List<Reservation>> GetPastReservationsAsync(string userId);
}
