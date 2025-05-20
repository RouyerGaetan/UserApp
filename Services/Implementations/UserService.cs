using System.Security.Claims;
using System.Threading.Tasks;
using UserApp.Models;
using UserApp.Repositories.Interfaces;
using UserApp.Services.Interfaces;

namespace UserApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            if (userPrincipal == null || !userPrincipal.Identity.IsAuthenticated)
                return null;

            var userId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return null;

            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User?> GetByIdAsync(string userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }
    }
}
