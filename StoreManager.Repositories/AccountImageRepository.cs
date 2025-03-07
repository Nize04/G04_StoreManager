using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class AccountImageRepository : RepositoryBase<AccountImage>, IAccountImageRepository
    {
        public AccountImageRepository(IDbConnection connection, IDbTransaction? transaction = null) : base(connection, transaction)
        {
        }

        protected override string[] UnwantedPropertiesForInsert => new string[] { "Id", "IsActive", "UploadedDate" };
        protected override string[] UnwantedPropertiesForUpdate => new string[] { "IsActive", "UploadedDate" };
    }
}