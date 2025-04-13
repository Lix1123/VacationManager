using System.Collections.Generic;
using System.Threading.Tasks;
using VacationManager.Models;

namespace VacationManager.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task CreateUserAsync(User user, string password);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string id);
    }
}
