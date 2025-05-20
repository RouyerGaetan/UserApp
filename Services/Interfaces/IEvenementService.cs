using UserApp.Models;

namespace UserApp.Services.Interfaces
{
    public interface IEvenementService
    {
        Task<Evenement?> GetEvenementByIdAsync(int id);

        Task<(List<Evenement>, int totalPages)> GetEvenementsFilteredAsync(
            string searchTerm,
            string sport,
            string ville,
            decimal? prixMax,
            DateTime? date,
            string filtreDate,
            int page,
            int pageSize);

        Task<bool> EstOrganisateurDeLEvenementAsync(
            int evenementId,
            string userId);

        Task CreateEvenementAsync(Evenement evenement);

        Task UpdateEvenementAsync(
            Evenement evenement,
            Evenement updatedEvent);

        Task DeleteEvenementAsync(int id);
    }
}
