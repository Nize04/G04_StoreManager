using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Repositories
{
    public interface ITokenRepository :IRepository<Token>
    {
        Task RevokeTokenAsync(string tokenString);
        Task<bool> IsTokenValidAsync(string tokenString);
        Task<Token?> GetByRefreshToken(string refreshToken);
    }
}