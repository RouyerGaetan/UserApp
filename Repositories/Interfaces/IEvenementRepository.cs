using UserApp.Models;

namespace UserApp.Repositories.Interfaces
{
    public interface IEvenementRepository
    {
        Task<Evenement?> GetByIdAsync(int id);

        Task<List<Evenement>> GetAllAsync();

        Task<(List<Evenement>, int totalPages)> GetFilteredAsync(
            string searchTerm,
            string sport,
            string ville,
            decimal? prixMax,
            DateTime? date,
            string filtreDate,
            int page,
            int pageSize);

        Task AddAsync(Evenement evenement);

        Task UpdateAsync(Evenement evenement);

        Task DeleteAsync(Evenement evenement);

        Task<bool> EstOrganisateurAsync(
            int evenementId,
            string userId);
    }
}
