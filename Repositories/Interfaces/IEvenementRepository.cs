using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.Repositories.Interfaces
{
    public interface IEvenementRepository
    {
        Task<Evenement?> GetByIdAsync(int id);
        Task<IEnumerable<Evenement>> GetAllAsync();
        Task<IEnumerable<Evenement>> GetFilteredAsync(string? searchTerm, string? sport, string? ville, decimal? prixMax, DateTime? date, string? filtreDate, int page, int pageSize);
        Task<int> GetCountFilteredAsync(string? searchTerm, string? sport, string? ville, decimal? prixMax, DateTime? date, string? filtreDate);
        Task AddAsync(Evenement evenement);
        Task UpdateAsync(Evenement evenement);
        Task DeleteAsync(Evenement evenement);
        Task<bool> IsOwnerAsync(int evenementId, string userId);
    }
}
