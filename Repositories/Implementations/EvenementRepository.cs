using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // Ici on peut garder une version simple sans Include
            return await _context.Evenements
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Evenement?> GetByIdWithClubAsync(int id)
        {
            return await _context.Evenements
                .AsNoTracking()
                .Include(e => e.Club)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Evenement>> GetAllAsync()
        {
            return await _context.Evenements
                .AsNoTracking()
                .Include(e => e.Club)
                    .ThenInclude(c => c.User)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evenement>> GetFilteredAsync(string? searchTerm, string? sport, string? ville, decimal? prixMax, DateTime? date, string? filtreDate, int page, int pageSize)
        {
            var query = _context.Evenements
                .AsNoTracking()
                .Include(e => e.Club)
                    .ThenInclude(c => c.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var loweredSearch = searchTerm.ToLower();
                query = query.Where(e => e.Titre.ToLower().Contains(loweredSearch));
            }

            if (!string.IsNullOrEmpty(sport))
            {
                var loweredSport = sport.ToLower();
                query = query.Where(e => e.Sport.ToLower() == loweredSport);
            }

            if (!string.IsNullOrEmpty(ville))
            {
                var loweredVille = ville.ToLower();
                query = query.Where(e => e.Ville.ToLower() == loweredVille);
            }

            if (prixMax.HasValue)
                query = query.Where(e => e.Prix <= prixMax);

            if (date.HasValue)
                query = query.Where(e => e.Date.Date == date.Value.Date);

            if (filtreDate == "avenir")
                query = query.Where(e => e.Date > DateTime.Now);
            else if (filtreDate == "passe")
                query = query.Where(e => e.Date < DateTime.Now);

            return await query
                .OrderBy(e => e.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<IEnumerable<Evenement>> GetEvenementsByClubIdAsync(int clubId)
        {
            return await _context.Evenements
                .AsNoTracking()
                .Where(e => e.ClubId == clubId)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<int> GetCountFilteredAsync(string? searchTerm, string? sport, string? ville, decimal? prixMax, DateTime? date, string? filtreDate)
        {
            var query = _context.Evenements
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var loweredSearch = searchTerm.ToLower();
                query = query.Where(e => e.Titre.ToLower().Contains(loweredSearch));
            }

            if (!string.IsNullOrEmpty(sport))
            {
                var loweredSport = sport.ToLower();
                query = query.Where(e => e.Sport.ToLower() == loweredSport);
            }

            if (!string.IsNullOrEmpty(ville))
            {
                var loweredVille = ville.ToLower();
                query = query.Where(e => e.Ville.ToLower() == loweredVille);
            }

            if (prixMax.HasValue)
                query = query.Where(e => e.Prix <= prixMax);

            if (date.HasValue)
                query = query.Where(e => e.Date.Date == date.Value.Date);

            if (filtreDate == "avenir")
                query = query.Where(e => e.Date > DateTime.Now);
            else if (filtreDate == "passe")
                query = query.Where(e => e.Date < DateTime.Now);

            return await query.CountAsync();
        }

        public async Task AddAsync(Evenement evenement)
        {
            await _context.Evenements.AddAsync(evenement);
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

        public async Task<bool> IsOwnerAsync(int evenementId, string userId)
        {
            // Vérifie si le userId correspond au UserId du Club organisateur de l'événement
            return await _context.Evenements
                .AsNoTracking()
                .Include(e => e.Club)
                .AnyAsync(e => e.Id == evenementId && e.Club.UserId == userId);
        }
    }
}
