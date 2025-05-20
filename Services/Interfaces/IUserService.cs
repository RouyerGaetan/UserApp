using System.Security.Claims;
using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Récupère l'utilisateur courant à partir du ClaimsPrincipal.
        /// </summary>
        Task<User?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);

        /// <summary>
        /// Récupère un utilisateur par son id.
        /// </summary>
        Task<User?> GetByIdAsync(string userId);
    }
}
