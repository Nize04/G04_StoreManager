
using StoreManager.DTO;
using StoreManager.Models;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IAccountService
    {
        Task<object> RegisterAsync(Account item);
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account?> GetAccountByIdAsync(int id);
        Task UpdateAccount(Account account);
        Task<int> UploadImageAsync(AccountImage accountImage);
        Task<IEnumerable<AccountImage>> GetImagesAsync();
        Task<LoginResult> ProcessLoginAsync(string email, string password, string clientKey);
        TwoFAResult Verify2FACodeAsync(string email, string code);
    }
}