using StoreManager.Models;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ITwoFactorAuthService
    {
        (string?, TwoFAResult) Verify2FACode(string code);
        Task<bool> Send2FACodeAsync(string email);
    }
}
