using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class SupplierTransactionRepository : RepositoryBase<SupplierTransaction>, ISupplierTransactionRepository
    {
        public SupplierTransactionRepository(IDbConnection connection, IDbTransaction? transaction = null) : base(connection, transaction)
        {
        }

        protected override string[] UnwantedPropertiesForInsert => new string[] { "IsActive", "TransactionTime" };
    }
}