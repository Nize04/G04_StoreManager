using Dapper;
using StoreManager.DTO;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Repositories;
using System.Data;

namespace StoreManager.Repositories
{
    internal class TokenRepository : RepositoryBase<Token>, ITokenRepository
    {
        public TokenRepository(IDbConnection connection, IDbTransaction? transaction = null) : base(connection, transaction)
        {
        }

        public async Task<bool> IsTokenValidAsync(string tokenString)
        {
            string sqlCommand = $"sp_GetTokenValidation";
            DynamicParameters parameters = new DynamicParameters(new { Token = tokenString });
            parameters.Add("@IsTokenValid", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<bool>("@IsTokenValid");
        }

        public async Task RevokeTokenAsync(string tokenString)
        {
            if (tokenString == null) throw new ArgumentNullException();

            string sqlCommand = $"sp_RevokeToken";
            DynamicParameters parameters = new DynamicParameters();
            byte[] tokenHash = tokenString.HashToken();
            parameters.Add("@TokenHash", tokenHash, DbType.Binary);
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure);
        }

        protected override string[] UnwantedPropertiesForInsert => new string[] { "IsActive", "CreateDate", "RevokedAt", "Id" };
    }
}