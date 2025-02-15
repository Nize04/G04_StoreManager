
using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IRoleService
    {
        Task<int> AddRoleAsync(Role role);
        Task AssignRoleToAccountAsync(AccountRole accountRole);
        Task<IEnumerable<Role>> GetRolesByAccountIdAsync(int accountId);
        Task RemoveRoleFromAccountAsync(int accountId, int roleId);
    }
}