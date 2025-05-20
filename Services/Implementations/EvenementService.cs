using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserApp.Models;
using UserApp.Repositories.Interfaces;
using UserApp.Services.Interfaces;

namespace UserApp.Services
{
    public class EvenementService : IEvenementService
    {
        private readonly IEvenementRepository _evenementRepository;

        public EvenementService(IEvenementRepository evenementRepository)
        {
            _evenementRepository = evenementRepository;
        }

        public async Task<Evenement?> GetEvenementByIdAsync(int id)
        {
            return await _evenementRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Evenement>> GetAllAsync()
        {
            return await _evenementRepository.GetAllAsync();
        }

        public async Task<PagedResult<Evenement>> GetEvenementsWithFilterAsync(string? searchTerm, string? sport, string? ville, decimal? prixMax, DateTime? date, string? filtreDate, int page, int pageSize)
        {
            var items = await _evenementRepository.GetFilteredAsync(searchTerm, sport, ville, prixMax, date, filtreDate, page, pageSize);
            var totalCount = await _evenementRepository.GetCountFilteredAsync(searchTerm, sport, ville, prixMax, date, filtreDate);

            return new PagedResult<Evenement>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        // --- Mise à jour ici : retour OperationResult et gestion des erreurs ---
        public async Task<OperationResult> AddEvenementAsync(Evenement evenement)
        {
            if (evenement == null)
                return OperationResult.Fail("Evenement", "L'événement ne peut pas être nul.");

            evenement.AvailableSeats = evenement.TotalSeats;

            await _evenementRepository.AddAsync(evenement);
            return OperationResult.Success();
        }

        public async Task<OperationResult> UpdateEvenementAsync(Evenement updatedEvent)
        {
            if (updatedEvent == null)
                return OperationResult.Fail("Evenement", "L'événement ne peut pas être nul.");

            var existingEvent = await _evenementRepository.GetByIdAsync(updatedEvent.Id);
            if (existingEvent == null)
                return OperationResult.Fail("Evenement", "L'événement n'existe pas.");

            int reservedSeats = existingEvent.TotalSeats - existingEvent.AvailableSeats;
            int availableSeats = updatedEvent.TotalSeats - reservedSeats;

            if (availableSeats < 0)
            {
                return OperationResult.Fail("TotalSeats", "Le nombre total de places ne peut pas être inférieur au nombre de places déjà réservées.");
            }

            existingEvent.Titre = updatedEvent.Titre;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.Ville = updatedEvent.Ville;
            existingEvent.Date = updatedEvent.Date;
            existingEvent.Sport = updatedEvent.Sport;
            existingEvent.Prix = updatedEvent.Prix;
            existingEvent.ImageUrl = updatedEvent.ImageUrl;
            existingEvent.TotalSeats = updatedEvent.TotalSeats;
            existingEvent.AvailableSeats = availableSeats;

            await _evenementRepository.UpdateAsync(existingEvent);
            return OperationResult.Success();
        }

        public async Task<OperationResult> DeleteEvenementAsync(int id, string userId)
        {
            var evenement = await _evenementRepository.GetByIdAsync(id);
            if (evenement == null)
                return OperationResult.Fail("Evenement", "L'événement n'existe pas.");

            var isOwner = await IsUserOwnerOfEventAsync(id, userId);
            if (!isOwner)
                return OperationResult.Fail("Authorization", "Vous n'êtes pas autorisé à supprimer cet événement.");

            await _evenementRepository.DeleteAsync(evenement);
            return OperationResult.Success();
        }

        public async Task<bool> IsUserOwnerOfEventAsync(int evenementId, string userId)
        {
            return await _evenementRepository.IsOwnerAsync(evenementId, userId);
        }
    }
}
