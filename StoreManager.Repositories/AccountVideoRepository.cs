using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class AccountVideoRepository : RepositoryBase<AccountVideo>, IAccountVideoRepository
    {
        public AccountVideoRepository(IDbConnection connection, IDbTransaction? transaction) : base(connection, transaction)
        {
        }

        protected override string[] UnwantedPropertiesForInsert => new string[] { "Id", "IsActive", "UploadedDate" };
        protected override string[] UnwantedPropertiesForUpdate => new string[] { "IsActive", "UploadedDate" };
    }
}