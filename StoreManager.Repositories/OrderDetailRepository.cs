using Dapper;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(IDbConnection connection, IDbTransaction? dbTransaction) : base(connection, dbTransaction) { }

        public async override Task<object> InsertAsync(OrderDetail item)
        {
            string sqlCommand = $"sp_InsertOrderDetail";
            DynamicParameters parameters = new DynamicParameters(GetParameters(item, _unwantedPropertiesForInsert));
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure,
                transaction: _transaction);

            return -1;
        }
    }
}