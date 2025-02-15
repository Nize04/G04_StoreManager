using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Facade.Interfaces.Trackers;
using StoreManager.Models;

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

        var cacheKey = $"2fa-{email}";
        _cache.Set(cacheKey, twoFACode, TimeSpan.FromMinutes(5));

        var subject = "Your 2FA Code";
        var body = $"Your 2FA code is: {twoFACode}";

        bool emailSent = await _emailSenderService.SendEmailAsync(email, subject, body);
        return emailSent;
    }

    public TwoFAResult Verify2FACode(string email, string code)
    {
        var cacheKey = $"2fa-{email}";

        if (_tracker.Is2FAVerificationLockedOut(cacheKey))
        {
            _logger.LogWarning("2FA verification locked out for Email: {Email}", email);
            return TwoFAResult.LockedOut;
        }

        if (_cache.TryGetValue(cacheKey, out string storedCode))
        {
            if (storedCode == code)
            {
                _cache.Remove(cacheKey);
                _logger.LogInformation("Succesfuly verify code {code}", code);
                return TwoFAResult.Success;
            }
        }

        _logger.LogInformation("Unsuccesfuly verify code {code}", code);
        _tracker.Register2FAVerificationFailedAttempt(cacheKey);
        return TwoFAResult.InvalidCode;
    }

    private string Generate2FACode()
    {
        var random = new Random();
        var code = random.Next(100000, 999999).ToString();
        return code;
    }
}