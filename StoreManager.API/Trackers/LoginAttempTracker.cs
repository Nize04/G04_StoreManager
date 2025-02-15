using StoreManager.Facade.Interfaces.Trackers;
using System.Collections.Concurrent;

public class LoginAttemptTracker : ILoginAttemptTracker
{
    private readonly ConcurrentDictionary<string, AttemptInfo> _attempts = new();
    private readonly int _maxLoginAttempts = 5;
    private readonly int _max2FAVerifications = 5;
    private readonly TimeSpan _lockoutDuration = TimeSpan.FromMinutes(5);

    private class AttemptInfo
    {
        public int LoginAttempts { get; set; } = 0;
        public DateTime LoginLockoutEnd { get; set; } = DateTime.MinValue;

        public int TwoFactorVerifications { get; set; } = 0;
        public DateTime TwoFactorVerificationLockoutEnd { get; set; } = DateTime.MinValue;
    }

    private AttemptInfo GetOrCreate(string key)
    {
        return _attempts.GetOrAdd(key, _ => new AttemptInfo());
    }

    public bool IsLoginLockedOut(string key)
    {
        var info = GetOrCreate(key);
        return DateTime.UtcNow < info.LoginLockoutEnd;
    }

    public void RegisterLoginFailedAttempt(string key)
    {
        var info = GetOrCreate(key);
        info.LoginAttempts++;
        if (info.LoginAttempts >= _maxLoginAttempts)
        {
            info.LoginLockoutEnd = DateTime.UtcNow.Add(_lockoutDuration);
        }
    }

    public int GetRemainingLoginAttempts(string key)
    {
        var info = GetOrCreate(key);
        return Math.Max(0, _maxLoginAttempts - info.LoginAttempts);
    }

    public void ResetLoginAttempts(string key)
    {
        var info = GetOrCreate(key);
        info.LoginAttempts = 0;
        info.LoginLockoutEnd = DateTime.MinValue;
    }

    public bool Is2FAVerificationLockedOut(string key)
    {
        var info = GetOrCreate($"{key}:2FA:Verify");
        return DateTime.UtcNow < info.TwoFactorVerificationLockoutEnd;
    }

    public void Register2FAVerificationFailedAttempt(string key)
    {
        var info = GetOrCreate($"{key}:2FA:Verify");
        info.TwoFactorVerifications++;
        if (info.TwoFactorVerifications >= _max2FAVerifications)
        {
            info.TwoFactorVerificationLockoutEnd = DateTime.UtcNow.Add(_lockoutDuration);
        }
    }

    public int GetRemaining2FAVerificationAttempts(string key)
    {
        var info = GetOrCreate($"{key}:2FA:Verify");
        return Math.Max(0, _max2FAVerifications - info.TwoFactorVerifications);
    }

    public void Reset2FAVerificationAttempts(string key)
    {
        var info = GetOrCreate($"{key}:2FA:Verify");
        info.TwoFactorVerifications = 0;
        info.TwoFactorVerificationLockoutEnd = DateTime.MinValue;
    }
}