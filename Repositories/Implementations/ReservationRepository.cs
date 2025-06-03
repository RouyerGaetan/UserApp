using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reservation>> GetReservationsByUserIdAsync(string userId)
    {
        return await _context.Reservations
            .Include(r => r.Evenement)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Reservation>> GetPastReservationsByUserIdAsync(string userId)
    {
        var now = DateTime.Now;
        return await _context.Reservations
            .Include(r => r.Evenement)
                .ThenInclude(e => e.NoteEvenements)
            .Where(r => r.UserId == userId && r.Evenement != null && r.Evenement.Date < now)
            .ToListAsync();
    }

    public async Task<Reservation?> GetExistingReservationAsync(int evenementId, string userId)
    {
        return await _context.Reservations
            .FirstOrDefaultAsync(r => r.EvenementId == evenementId && r.UserId == userId);
    }

    public async Task<Evenement?> GetEvenementByIdAsync(int evenementId)
    {
        return await _context.Evenements.FindAsync(evenementId);
    }

    public async Task AddReservationAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetTotalReservedSeatsAsync(int evenementId, string userId)
    {
        return await _context.Reservations
            .Where(r => r.EvenementId == evenementId && r.UserId == userId)
            .SumAsync(r => r.NumberOfSeats);
    }

    public async Task UpdateEvenementAsync(Evenement evenement)
    {
        _context.Evenements.Update(evenement);
        await Task.CompletedTask;
    }

    public async Task<Reservation?> GetReservationByIdAsync(int reservationId)
    {
        return await _context.Reservations
            .Include(r => r.Evenement)
            .FirstOrDefaultAsync(r => r.Id == reservationId);
    }

    public async Task DeleteReservationAsync(Reservation reservation)
    {
        _context.Reservations.Remove(reservation);
        await Task.CompletedTask;
    }

    // ✅ Nouvelle méthode : Récupère toutes les réservations pour les événements créés par un organisateur
    public async Task<List<Reservation>> GetReservationsForOrganisateurAsync(string userId)
    {
        var clubIds = await _context.Clubs
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .ToListAsync();

        var reservations = await _context.Reservations
            .Include(r => r.Evenement)
            .Include(r => r.User) // <-- Ajout ici pour charger l'utilisateur lié
            .Where(r => r.Evenement != null && clubIds.Contains(r.Evenement.ClubId))
            .ToListAsync();

        return reservations;
    }
}
