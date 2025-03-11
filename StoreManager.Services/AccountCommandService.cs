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

            await _unitOfWork.OpenConnectionAsync();
            try
            {
                return await _unitOfWork.AccountRepository.InsertAsync(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed EmployeeId: {EmployeeId}", account.Id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
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
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account), "Account cannot be null.");
            }

            string ipAddress = _userRequestHelper.GetUserIpAddress()!;
            string deviceInfo = _userRequestHelper.GetDeviceDetails();

            try
            {
                var clientInfo = _userRequestHelper.GetClientInfoFromDeviceInfo(deviceInfo);

                if (SecurityHelper.IsKnownBot(clientInfo))
                {
                    _logger.LogWarning("⚠️ Bot detected! Blocking authorization attempt. IP: {IpAddress}, Device Info: {DeviceInfo}", ipAddress, deviceInfo);
                    throw new SecurityException("Authorization blocked due to bot activity.");
                }

                if (SecurityHelper.IsSuspiciousIp(ipAddress))
                {
                    _logger.LogWarning("⚠️ Suspicious IP detected! Additional verification needed. IP: {IpAddress}", ipAddress);
                    throw new SecurityException("Authorization blocked due to suspicious IP address activity.");
                }

                var tokenResponse = _tokenService.GenerateTokenAsync(account);

                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    _logger.LogError("❌ Failed to generate token for account {UserId}, IP: {IpAddress}", account.Id, ipAddress);
                    throw new InvalidOperationException("Failed to generate authentication token.");
                }

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

                _logger.LogInformation("✅ Account successfully authorized. UserId: {UserId}, IP: {IpAddress}, Device Info: {DeviceInfo}",
                    account.Id, ipAddress, deviceInfo);
            }
            catch (SecurityException secEx)
            {
                _logger.LogError(secEx, "❌ Security issue during authorization: {Message}", secEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error during authorization process for UserId: {UserId}", account.Id);
                throw new InvalidOperationException("An error occurred while authorizing the account. Please try again.");
            }
        }

        public TwoFAResult Verify2FACode(string email, string code) =>
          _twoFactorAuthService.Verify2FACode(email, code);

        private async Task<bool> Send2FACodeAsync(string email) =>
            await _twoFactorAuthService.Send2FACodeAsync(email);

        public async Task UpdateAccount(Account account)
        {
            _logger.LogInformation("Updating Start EmployeeId: {EmployeeId}", account.Id);

            await _unitOfWork.OpenConnectionAsync();
            try
            {
                await _unitOfWork.AccountRepository.UpdateAsync(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Updating failed EmployeeId: {EmployeeId}", account.Id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }
    }
}
