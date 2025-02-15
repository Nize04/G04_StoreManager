using StoreManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ITwoFactorAuthService
    {
        TwoFAResult Verify2FACode(string email, string code);
        Task<bool> Send2FACodeAsync(string email);
    }
}
