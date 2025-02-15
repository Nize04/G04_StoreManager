using StoreManager.DTO;
using StoreManager.Models;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ITokenService
    {
        Task<int> InsertAsync(Token token);
        Task RevokeTokenAsync(string tokenString);
        TokenResponse GenerateToken(Account account);
        Task<bool> IsTokenValidAsync(string tokenString);
    }
}
