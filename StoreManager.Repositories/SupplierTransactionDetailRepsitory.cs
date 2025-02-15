using Dapper;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Repositories
{
    internal class SupplierTransactionDetailRepsitory : RepositoryBase<SupplierTransactionDetail>, ISupplierTransactionDetailRepository
    {
        public SupplierTransactionDetailRepsitory(IDbConnection connection, IDbTransaction? transaction = null) : base(connection, transaction)
        {
        }
        public async override Task<object> InsertAsync(SupplierTransactionDetail item)
        {
            string sqlCommand = $"sp_InsertSupplierTransactionDetail";
            DynamicParameters parameters = new DynamicParameters(GetParameters(item, _unwantedPropertiesForInsert));
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure,
                transaction: _transaction);

            return -1;
        }
        protected override string[] UnwantedPropertiesForInsert => new string[] { "IsActive" };
    }
}
