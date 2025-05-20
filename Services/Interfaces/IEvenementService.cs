using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.Services.Interfaces
{
    public interface IEvenementService
    {
        Task<Evenement?> GetEvenementByIdAsync(int id);
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

        Task<OperationResult> AddEvenementAsync(Evenement evenement);   // <-- retour OperationResult

        Task<OperationResult> UpdateEvenementAsync(Evenement updatedEvent);  // <-- retour OperationResult

        Task<OperationResult> DeleteEvenementAsync(int id, string userId);  // <-- retour OperationResult

        Task<bool> IsUserOwnerOfEventAsync(int evenementId, string userId);
    }
}
