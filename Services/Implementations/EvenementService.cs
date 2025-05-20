using UserApp.Models;
using UserApp.Repositories.Interfaces;
using UserApp.Services.Interfaces;

namespace UserApp.Services
{
    public class EvenementService : IEvenementService
    {
        private readonly IEvenementRepository _repository;

        public EvenementService(IEvenementRepository repository)
        {
            _repository = repository;
        }

        public async Task<Evenement?> GetEvenementByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<(List<Evenement>, int totalPages)> GetEvenementsFilteredAsync(
            string searchTerm,
            string sport,
            string ville,
            decimal? prixMax,
            DateTime? date,
            string filtreDate,
            int page,
            int pageSize)
        {
            return await _repository.GetFilteredAsync(
                searchTerm,
                sport,
                ville,
                prixMax,
                date,
                filtreDate,
                page,
                pageSize);
        }

        public async Task<bool> EstOrganisateurDeLEvenementAsync(
            int evenementId,
            string userId)
        {
            return await _repository.EstOrganisateurAsync(evenementId, userId);
        }

        public async Task CreateEvenementAsync(Evenement evenement)
        {
            await _repository.AddAsync(evenement);
        }

        public async Task UpdateEvenementAsync(
            Evenement evenement,
            Evenement updatedEvent)
        {
            evenement.Titre = updatedEvent.Titre;
            evenement.Description = updatedEvent.Description;
            evenement.Sport = updatedEvent.Sport;
            evenement.Ville = updatedEvent.Ville;
            evenement.Date = updatedEvent.Date;
            evenement.Prix = updatedEvent.Prix;
            evenement.TotalSeats = updatedEvent.TotalSeats;
            evenement.AvailableSeats = updatedEvent.AvailableSeats;
            evenement.ImageUrl = updatedEvent.ImageUrl;

            await _repository.UpdateAsync(evenement);
        }

        public async Task DeleteEvenementAsync(int id)
        {
            var evenement = await _repository.GetByIdAsync(id);
            if (evenement != null)
            {
                await _repository.DeleteAsync(evenement);
            }
        }
    }
}
