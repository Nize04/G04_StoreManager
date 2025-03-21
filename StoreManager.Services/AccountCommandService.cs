using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Facade.Interfaces.Trackers;
using StoreManager.Facade.Interfaces.Utilities;
using StoreManager.Models;
using System.Security;

namespace StoreManager.Services
{
    public class AccountCommandService : IAccountCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountCommandService> _logger;
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly ITokenService _tokenService;
        private readonly IUserRequestHelper _userRequestHelper;
        private readonly ILoginAttemptTracker _loginAttemptTracker;

        public AccountCommandService(IUnitOfWork unitOfWork,
            ILogger<AccountCommandService> logger,
            ITwoFactorAuthService twoFactorAuthService,
            ITokenService tokenService,
            IUserRequestHelper userRequestHelper,
            ILoginAttemptTracker loginAttemptTracker)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _twoFactorAuthService = twoFactorAuthService ?? throw new ArgumentNullException(nameof(twoFactorAuthService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userRequestHelper = userRequestHelper ?? throw new ArgumentNullException(nameof(userRequestHelper));
            _loginAttemptTracker = loginAttemptTracker ?? throw new ArgumentNullException(nameof(loginAttemptTracker));
        }


        public async Task<object> RegisterAsync(Account account)
        {
            _logger.LogInformation("Registration Start EmployeeId: {EmployeeId}", account.Id);

            try
            {
                if (!PasswordHelper.CheckPasswordRequirements(account.Password))
                {
                    _logger.LogError("❌ Password validation failed. Password does not meet security requirements.");
                    throw new SecurityException("Password does not meet security requirements.");
                }

                var (hash, salt) = PasswordHelper.HashPassword(account.Password);

                account.Password = hash;
                account.Salt = salt;
                return await _unitOfWork.AccountRepository.InsertAsync(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed EmployeeId: {EmployeeId}", account.Id);
                throw;
            }
        }

        public async Task EnableTwoFactorAuthAsync(int accountId, string password)
        {
            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);

                if (account == null)
                {
                    _logger.LogWarning("⚠️ Attempt to enable 2FA for non-existent account. AccountId: {AccountId}", accountId);
                    throw new KeyNotFoundException("Account not found.");
                }

                if (account.Requires2FA)
                {
                    _logger.LogWarning("⚠️ 2FA already enabled for AccountId: {AccountId}", accountId);
                    throw new InvalidOperationException("This account already has 2FA enabled.");
                }

                if (!PasswordHelper.ValidatePassword(password, account.Password, account.Salt))
                {
                    _logger.LogWarning("⚠️ Invalid password attempt for enabling 2FA. AccountId: {AccountId}", accountId);
                    throw new UnauthorizedAccessException("Invalid password.");
                }

                account.Requires2FA = true;
                await _unitOfWork.AccountRepository.UpdateAsync(account);

                _logger.LogInformation("✅ 2FA successfully enabled for AccountId: {AccountId}", accountId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enabling 2FA for AccountId: {AccountId}", accountId);
                throw;
            }
        }

        public async Task<LoginResult> ProcessLoginAsync(string email, string password, string clientKey)
        {
            // Attempt to retrieve the account associated with the provided email.
            var account = await _unitOfWork.AccountRepository.GetByEmailAsync(email);

            // If no account is found with the provided email, return an invalid credentials response.
            if (account == null)
            {
                _logger.LogWarning("Login failed for Email: {Email}. Account not found.", email);
                return new LoginResult { Status = LoginStatus.InvalidCredentials };
            }

            // Check if the user has been locked out due to too many failed login attempts.
            if (_loginAttemptTracker.IsLoginLockedOut(clientKey))
            {
                // If locked out, return a LockedOut status.
                return new LoginResult { Status = LoginStatus.LockedOut };
            }

            // Validate the entered password against the stored password and salt.
            if (!PasswordHelper.ValidatePassword(password, account.Password, account.Salt))
            {
                // If the password is incorrect, log the failed login attempt and register a failed attempt.
                _logger.LogWarning("Login failed for Email: {Email}. Incorrect password.", email);
                _loginAttemptTracker.RegisterLoginFailedAttempt(clientKey);

                // Return an invalid credentials response.
                return new LoginResult { Status = LoginStatus.InvalidCredentials };
            }

            // If the account requires two-factor authentication (2FA), initiate the 2FA process.
            if (account.Requires2FA)
            {
                // Attempt to send the 2FA code to the user's email.
                if (!await Send2FACodeAsync(email))
                {
                    // If sending the 2FA code fails, log the error and return a failure status.
                    _logger.LogError("Failed to send 2FA code for Email: {Email}", account.Email);
                    return new LoginResult { Status = LoginStatus.Failed2FASending };
                }

                // If 2FA is required, return a Requires2FA status along with the account details.
                return new LoginResult { Status = LoginStatus.Requires2FA, Account = account };
            }

            // If login is successful and no 2FA is required, reset any failed login attempts for the client.
            _loginAttemptTracker.ResetLoginAttempts(clientKey);

            // Return a Success status along with the authenticated account.
            return new LoginResult { Status = LoginStatus.Success, Account = account };
        }

        public async Task AuthorizeAccountAsync(Account account)
        {
            // Ensure the account parameter is not null.
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account), "Account cannot be null.");
            }

            // Retrieve the user's IP address and device information.
            string ipAddress = _userRequestHelper.GetUserIpAddress()!;
            string deviceInfo = _userRequestHelper.GetDeviceDetails();

            try
            {
                // Extract client information from the device details.
                var clientInfo = _userRequestHelper.GetClientInfoFromDeviceInfo(deviceInfo);

                // Check if the request is from a known bot.
                if (SecurityHelper.IsKnownBot(clientInfo))
                {
                    // Log the bot activity and throw an exception to block the authorization.
                    _logger.LogWarning("⚠️ Bot detected! Blocking authorization attempt. IP: {IpAddress}, Device Info: {DeviceInfo}", ipAddress, deviceInfo);
                    throw new SecurityException("Authorization blocked due to bot activity.");
                }

                // Check if the IP address is suspicious (e.g., flagged as malicious or unusual).
                if (SecurityHelper.IsSuspiciousIp(ipAddress))
                {
                    // Log the suspicious IP and throw an exception to block the authorization.
                    _logger.LogWarning("⚠️ Suspicious IP detected! Additional verification needed. IP: {IpAddress}", ipAddress);
                    throw new SecurityException("Authorization blocked due to suspicious IP address activity.");
                }

                // Generate an authentication token for the user account.
                var tokenResponse = _tokenService.GenerateTokenAsync(account);

                // Check if the token generation failed or the token is invalid.
                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    // Log the failure to generate the token and throw an exception.
                    _logger.LogError("❌ Failed to generate token for account {UserId}, IP: {IpAddress}", account.Id, ipAddress);
                    throw new InvalidOperationException("Failed to generate authentication token.");
                }

                // Store the generated token in the database, including relevant metadata like IP address and device info.
                await _tokenService.InsertAsync(new Token
                {
                    AccountId = tokenResponse.AccountId,
                    AccessTokenHash = tokenResponse.AccessToken.HashToken(),
                    RefreshToken = tokenResponse.RefreshToken,
                    AccessTokenExpiresAt = tokenResponse.AccessTokenExpiresAt,
                    RefreshTokenExpiresAt = tokenResponse.RefreshTokenExpiresAt,
                    IpAddress = ipAddress,
                    DeviceInfo = deviceInfo,
                    CreateDate = DateTime.UtcNow
                });

                // Log the successful authorization.
                _logger.LogInformation("✅ Account successfully authorized. UserId: {UserId}, IP: {IpAddress}, Device Info: {DeviceInfo}",
                    account.Id, ipAddress, deviceInfo);
            }
            catch (SecurityException secEx)
            {
                // Log security-related exceptions and rethrow them.
                _logger.LogError(secEx, "❌ Security issue during authorization: {Message}", secEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                // Log any other exceptions and throw a general error message.
                _logger.LogError(ex, "❌ Error during authorization process for UserId: {UserId}", account.Id);
                throw new InvalidOperationException("An error occurred while authorizing the account. Please try again.");
            }
        }

        public (string?,TwoFAResult) Verify2FACode(string code) =>
          _twoFactorAuthService.Verify2FACode(code);

        private async Task<bool> Send2FACodeAsync(string email) =>
            await _twoFactorAuthService.Send2FACodeAsync(email);

        public async Task<bool> ChangePasswordAsync(int accountId, string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException("Passwords cannot be empty.");
            }

            try
            {
                var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
                if (account == null)
                {
                    _logger.LogWarning("❌ Change password failed. Account not found. AccountId: {AccountId}", accountId);
                    throw new UnauthorizedAccessException("Invalid account.");
                }

                if (!PasswordHelper.ValidatePassword(oldPassword, account.Password, account.Salt))
                {
                    _logger.LogWarning("❌ Incorrect old password. AccountId: {AccountId}", accountId);
                    throw new UnauthorizedAccessException("Old password is incorrect.");
                }

                if (!PasswordHelper.CheckPasswordRequirements(newPassword))
                {
                    _logger.LogError("❌ Password validation failed. Password does not meet security requirements.");
                    throw new SecurityException("Password does not meet security requirements.");
                }

                if (oldPassword == newPassword)
                {
                    _logger.LogWarning("⚠️ New password cannot be the same as the old password. AccountId: {AccountId}", accountId);
                    throw new InvalidOperationException("New password must be different from the old password.");
                }

                var (hash, salt) = PasswordHelper.HashPassword(newPassword);

                account.Password = hash;
                account.Salt = salt;
                account.UpdateDate = DateTime.UtcNow;

                await _unitOfWork.AccountRepository.UpdateAsync(account);

                _logger.LogInformation("✅ Password changed successfully. AccountId: {AccountId}", accountId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while changing password process for AccountId: {AccountId}", accountId);
                throw new InvalidOperationException("An error occurred while authorizing the account. Please try again.");
            }
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _logger.LogInformation("Updating Start EmployeeId: {EmployeeId}", account.Id);

            try
            {
                await _unitOfWork.AccountRepository.UpdateAsync(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Updating failed EmployeeId: {EmployeeId}", account.Id);
                throw;
            }
        }
    }
}
