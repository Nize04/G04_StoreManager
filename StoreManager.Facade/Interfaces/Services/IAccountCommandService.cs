using StoreManager.DTO;
using StoreManager.Models;

public interface IAccountCommandService
{
    Task<object> RegisterAsync(Account account);
    Task UpdateAccount(Account account);
    Task<LoginResult> ProcessLoginAsync(string email, string password, string clientKey);
    TwoFAResult Verify2FACode(string email, string code);
}
