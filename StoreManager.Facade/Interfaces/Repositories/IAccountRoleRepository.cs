using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Repositories
{
    public interface IAccountRoleRepository:IRepository<AccountRole>
    {
        Task<IEnumerable<Role>> GetRolesByAccountId(int id);
    }
}