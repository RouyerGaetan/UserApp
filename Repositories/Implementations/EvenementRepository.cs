using Microsoft.EntityFrameworkCore;
using UserApp.Data;
using UserApp.Models;
using UserApp.Repositories.Interfaces;

namespace UserApp.Repositories
{
    public class EvenementRepository : IEvenementRepository
    {
        private readonly AppDbContext _context;

        public EvenementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Evenement?> GetByIdAsync(int id)
        {
            return await _context.Evenements.FindAsync(id);
        }

        public async Task<List<Evenement>> GetAllAsync()
        {
            return await _context.Evenements.ToListAsync();
        }

        public async Task<(List<Evenement>, int totalPages)> GetFilteredAsync(
            string searchTerm,
            string sport,
            string ville,
            decimal? prixMax,
            DateTime? date,
            string filtreDate,
            int page,
            int pageSize)
        {
            var query = _context.Evenements.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e =>
                    (e.Titre != null && e.Titre.Contains(searchTerm)) ||
                    (e.Description != null && e.Description.Contains(searchTerm)) ||
                    (e.Ville != null && e.Ville.Contains(searchTerm)) ||
                    (e.Sport != null && e.Sport.Contains(searchTerm))
                );
            }

            if (!string.IsNullOrEmpty(sport))
            {
                query = query.Where(e => e.Sport != null && e.Sport.Contains(sport));
            }

            if (!string.IsNullOrEmpty(ville))
            {
                query = query.Where(e => e.Ville != null && e.Ville.Contains(ville));
            }

            if (prixMax.HasValue)
            {
                query = query.Where(e => e.Prix <= prixMax.Value);
            }

            if (date.HasValue)
            {
                switch (filtreDate?.ToLower())
                {
                    case "avant":
                        query = query.Where(e => e.Date < date.Value);
                        break;
                    case "apres":
                        query = query.Where(e => e.Date > date.Value);
                        break;
                    case "egale":
                        query = query.Where(e => e.Date.Date == date.Value.Date);
                        break;
                    default:
                        break;
                }
            }

            if (filtreDate == "à venir")
            {
                query = query.Where(e => e.Date >= DateTime.Today);
            }
            else if (filtreDate == "passés")
            {
                query = query.Where(e => e.Date < DateTime.Today);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (result, totalPages);
        }

        public async Task AddAsync(Evenement evenement)
        {
            _context.Evenements.Add(evenement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Evenement evenement)
        {
            _context.Evenements.Update(evenement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Evenement evenement)
        {
            _context.Evenements.Remove(evenement);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EstOrganisateurAsync(int evenementId, string userId)
        {
            var evenement = await _context.Evenements.FindAsync(evenementId);
            return evenement != null && evenement.UserId == userId;
        }
    }
}
