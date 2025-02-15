using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class RoleRepository : RepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(IDbConnection connection, IDbTransaction? transaction) : base(connection, transaction)
        {
        }

    }
}