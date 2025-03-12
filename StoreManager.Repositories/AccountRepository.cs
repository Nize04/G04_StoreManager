using Dapper;
using StoreManager.DTO;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal sealed class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(IDbConnection connection,IDbTransaction? dbTransaction) : base(connection, dbTransaction) { }

        public async override Task<object> InsertAsync(Account item)
        {
            string sqlCommand = $"sp_InsertAccount";
            DynamicParameters parameters = new DynamicParameters(item.GetParameters(_unwantedPropertiesForInsert));
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure);

            return item.Id;
        }

        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            string sqlCommand = $"sp_AuthenticateUser";
            DynamicParameters parameters = new DynamicParameters(new { Email = email, Password = password });
            parameters.Add("@IsAuthenticate", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<bool>("@IsAuthenticate");
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            IEnumerable<Account> accounts = await GetAsync(a => a.Email == email && a.IsActive == true);

            return accounts.FirstOrDefault();
        }
        protected override string[] UnwantedPropertiesForInsert => new[] { "CreateDate", "UpdateDate", "IsActive", "Requires2FA"};
        protected override string[] UnwantedPropertiesForUpdate => new[] { "CreateDate", "UpdateDate", "IsActive"};
    }
}
