using System.Collections.Generic;
using System.Threading.Tasks;
using UserApp.Models;

public interface IReservationRepository
{
    Task<List<Reservation>> GetReservationsByUserIdAsync(string userId);
    Task<Reservation?> GetExistingReservationAsync(int evenementId, string userId);
    Task<Evenement?> GetEvenementByIdAsync(int evenementId);
    Task AddReservationAsync(Reservation reservation);
    Task SaveChangesAsync();

    Task<int> GetTotalReservedSeatsAsync(int evenementId, string userId);
    Task UpdateEvenementAsync(Evenement evenement);

    // MODIF : Déclarations des nouvelles méthodes pour l'annulation
    Task<Reservation?> GetReservationByIdAsync(int reservationId);
    Task DeleteReservationAsync(Reservation reservation);
    Task<List<Reservation>> GetPastReservationsByUserIdAsync(string userId);
}
