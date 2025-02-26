using StoreManager.Models;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ITwoFactorAuthService
    {
        TwoFAResult Verify2FACode(string email, string code);
        Task<bool> Send2FACodeAsync(string email);
    }
}