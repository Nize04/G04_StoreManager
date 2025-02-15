using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class ProductsQuantityRepository : RepositoryBase<ProductsQuantity>, IProductQuantityRepository
    {
        public ProductsQuantityRepository(IDbConnection connection, IDbTransaction? dbTransaction) : base(connection, dbTransaction)
        {
        }

        protected override string[] UnwantedPropertiesForInsert => new string[] { "UpdateDate" };
    }
}