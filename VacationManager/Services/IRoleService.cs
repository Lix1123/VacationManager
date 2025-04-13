using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VacationManager.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task<IdentityRole?> GetRoleByIdAsync(string id);
        Task CreateRoleAsync(IdentityRole role);
        Task UpdateRoleAsync(IdentityRole role);
        Task DeleteRoleAsync(string id);
    }
}
