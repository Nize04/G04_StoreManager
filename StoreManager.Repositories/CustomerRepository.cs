using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal sealed class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(IDbConnection connection, IDbTransaction? dbTransaction) : base(connection, dbTransaction) { }
    }
}