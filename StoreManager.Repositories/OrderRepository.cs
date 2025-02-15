using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(IDbConnection connection, IDbTransaction? dbTransaction) : base(connection, dbTransaction) { }
    }
}