using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Facade.Interfaces.Trackers;
using StoreManager.Models;
using System.Security.Cryptography;

public class TwoFactorAuthService : ITwoFactorAuthService
{
    private readonly ILoginAttemptTracker _tracker;
    private readonly ILogger<TwoFactorAuthService> _logger;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IMemoryCache _cache;

    public TwoFactorAuthService(IEmailSenderService emailSenderService,
        IMemoryCache cache,
        ILoginAttemptTracker loginAttemptTracker,
        ILogger<TwoFactorAuthService> logger)
    {
        _emailSenderService = emailSenderService ?? throw new ArgumentNullException(nameof(emailSenderService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _tracker = loginAttemptTracker ?? throw new ArgumentNullException(nameof(loginAttemptTracker));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Send2FACodeAsync(string email)
    {
        var twoFACode = Generate2FACode();
        var cacheKey = $"2fa-code-{twoFACode}";

        _cache.Set(cacheKey, email, TimeSpan.FromMinutes(5));

        var subject = "Your 2FA Code";
        var body = $"Your 2FA code is: {twoFACode}";

        bool emailSent = await _emailSenderService.SendEmailAsync(email, subject, body);

        if (emailSent)
        {
            _logger.LogInformation("✅ 2FA code sent to {Email}.", email);
        }
        else
        {
            _logger.LogError("❌ Failed to send 2FA code to {Email}.", email);
        }

        return emailSent;
    }

    public (string?, TwoFAResult) Verify2FACode(string code)
    {
        var cacheKey = $"2fa-code-{code}";

        if (_tracker.Is2FAVerificationLockedOut(cacheKey))
        {
            _logger.LogWarning("🚫 2FA verification locked out for attempted code: {Code}", code);
            return (null, TwoFAResult.LockedOut);
        }

        if (_cache.TryGetValue(cacheKey, out string email))
        {
            _cache.Remove(cacheKey);
            _logger.LogInformation("✅ 2FA verification successful for Email: {Email}", email);
            return (email, TwoFAResult.Success);
        }

        _logger.LogWarning("❌ Invalid 2FA code attempt: {Code}", code);
        _tracker.Register2FAVerificationFailedAttempt(cacheKey);
        return (email, TwoFAResult.InvalidCode);
    }

    private string Generate2FACode()
    {
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        int code = BitConverter.ToInt32(bytes, 0) % 900000 + 100000;
        return Math.Abs(code).ToString();
    }
}
