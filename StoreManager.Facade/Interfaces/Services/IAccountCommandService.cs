using StoreManager.DTO;
using StoreManager.Models;

public interface IAccountCommandService
{
    Task<object> RegisterAsync(Account account);
    Task UpdateAccountAsync(Account account);
    Task<LoginResult> ProcessLoginAsync(string email, string password, string clientKey);
    (string?, TwoFAResult) Verify2FACode(string code);
    Task AuthorizeAccountAsync(Account account);
    Task<bool> ChangePasswordAsync(int accountId, string oldPassword, string newPassword);
    Task EnableTwoFactorAuthAsync(int accountId, string password);
}
