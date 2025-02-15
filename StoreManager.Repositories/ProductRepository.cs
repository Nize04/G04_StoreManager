using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal sealed class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IDbConnection connection, IDbTransaction? dbTransaction) : base(connection, dbTransaction) { }

        protected override string[] UnwantedPropertiesForInsert => new[] { "Id", "CreateDate", "UpdateDate", "IsActive","Description" };
        protected override string[] UnwantedPropertiesForUpdate => new[] { "CreateDate", "UpdateDate", "IsActive", "Description" };
    }
}