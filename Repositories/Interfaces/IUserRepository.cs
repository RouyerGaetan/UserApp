using System.Threading.Tasks;
using UserApp.Models;

namespace UserApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string userId);
    }
}
