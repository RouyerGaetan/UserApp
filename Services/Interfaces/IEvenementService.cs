using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.Services.Interfaces
{
    public interface IEvenementService
    {
        Task<Evenement?> GetEvenementByIdAsync(int id);

        // Ajout de la méthode pour récupérer un événement avec le Club inclus
        Task<Evenement?> GetEvenementByIdWithClubAsync(int id);

        Task<IEnumerable<Evenement>> GetAllAsync();

        Task<PagedResult<Evenement>> GetEvenementsWithFilterAsync(
            string? searchTerm,
            string? sport,
            string? ville,
            decimal? prixMax,
            DateTime? date,
            string? filtreDate,
            int page,
            int pageSize);
        Task<IEnumerable<Evenement>> GetEvenementsByClubIdAsync(int clubId);

        Task<OperationResult> AddEvenementAsync(Evenement evenement);

        Task<OperationResult> UpdateEvenementAsync(Evenement updatedEvent);

        Task<OperationResult> DeleteEvenementAsync(int id, string userId);

        Task<bool> IsUserOwnerOfEventAsync(int evenementId, string userId);
    }
}
