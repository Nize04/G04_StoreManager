using Dapper;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class AccountRoleRepository : RepositoryBase<AccountRole>, IAccountRoleRepository
    {
        public AccountRoleRepository(IDbConnection connection, IDbTransaction? transaction) : base(connection, transaction)
        {
        }

        public async override Task<object> InsertAsync(AccountRole item)
        {
            string sqlCommand = $"sp_InsertAccountRole";
            DynamicParameters parameters = new DynamicParameters(GetParameters(item, _unwantedPropertiesForInsert));;
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);

            return -1;
        }

        public async Task<IEnumerable<Role>> GetRolesByAccountId(int id)
        {
            DynamicParameters parameters = new DynamicParameters(new { Id = id });
            var roles = await _connection.QueryAsync<Role>(
                    "sp_GetRolesById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            return roles;
        }
    }
}