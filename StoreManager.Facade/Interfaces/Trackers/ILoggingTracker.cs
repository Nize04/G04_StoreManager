namespace StoreManager.Facade.Interfaces.Trackers
{
    public interface ILoginAttemptTracker
    {
        int GetRemainingLoginAttempts(string key);
        void ResetLoginAttempts(string key);
        bool Is2FAVerificationLockedOut(string key);
        bool IsLoginLockedOut(string key);
        void RegisterLoginFailedAttempt(string key);
        void Register2FAVerificationFailedAttempt(string key);
        int GetRemaining2FAVerificationAttempts(string key);
        void Reset2FAVerificationAttempts(string key);
    }
}