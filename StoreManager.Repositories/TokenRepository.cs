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
            DynamicParameters parameters = new DynamicParameters(new { AccessTokenHash = tokenString.HashToken() });
            parameters.Add("@IsTokenValid", dbType: DbType.Boolean, direction: ParameterDirection.Output);
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
            return parameters.Get<bool>("@IsTokenValid");
        }

        public async Task RevokeTokenAsync(string tokenString)
        {
            string sqlCommand = $"sp_RevokeToken";
            DynamicParameters parameters = new DynamicParameters();
            byte[] tokenHash = tokenString.HashToken();
            parameters.Add("@AccessTokenHash", tokenHash, DbType.Binary);
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        public async Task<Token?> GetByRefreshToken(string refreshToken)
        {
            string sqlCommand = $"sp_GetByRefreshToken";
            DynamicParameters parameters = new DynamicParameters(new { RefreshToken = refreshToken });
            return await _connection.QueryFirstOrDefaultAsync<Token>(sqlCommand, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        public override async Task UpdateAsync(Token item)
        {
            string sqlCommand = $"sp_UpdateToken";
            byte[] tokenHash = item.AccessTokenHash;
            DynamicParameters parameters = new DynamicParameters(new
            {
                item.Id,
                item.AccessTokenExpiresAt,
                item.
                RefreshToken,
                item.RefreshTokenExpiresAt,
                item.RevokedAt
            });
            parameters.Add("@AccessTokenHash", item.AccessTokenHash, DbType.Binary);
            await _connection.ExecuteAsync(sqlCommand, parameters, commandType: CommandType.StoredProcedure, transaction: _transaction);
        }

        protected override string[] UnwantedPropertiesForInsert => new string[] { "IsActive", "CreateDate", "RevokedAt", "Id" };
        protected override string[] UnwantedPropertiesForUpdate => new string[] { "IsActive", "CreateDate", "DeviceInfo", "AccountId" };
    }
}
