using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Facade.Interfaces.Trackers;
using StoreManager.Models;

namespace StoreManager.Services
{
    public class AccountCommandService : IAccountCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountCommandService> _logger;
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly ITokenService _tokenService;
        private readonly ILoginAttemptTracker _loginAttemptTracker;

        public AccountCommandService(IUnitOfWork unitOfWork,
            ILogger<AccountCommandService> logger,
            ITwoFactorAuthService twoFactorAuthService,
            ITokenService tokenService,
            ILoginAttemptTracker loginAttemptTracker)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _twoFactorAuthService = twoFactorAuthService ?? throw new ArgumentNullException(nameof(twoFactorAuthService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
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
            var account = await _unitOfWork.AccountRepository.GetByEmailAsync(email);
            if (account == null)
            {
                _logger.LogWarning("Login failed for Email: {Email}. Account not found.", email);
                return new LoginResult { Status = LoginStatus.InvalidCredentials };
            }

            if (!PasswordHelper.ValidatePassword(password, account.Password, account.Salt))
            {
                _logger.LogWarning("Login failed for Email: {Email}. Incorrect password.", email);
                return new LoginResult { Status = LoginStatus.InvalidCredentials };
            }

            if (_loginAttemptTracker.IsLoginLockedOut(clientKey))
            {
                return new LoginResult { Status = LoginStatus.LockedOut };
            }

            if (account.Requires2FA)
            {
                if (!await Send2FACodeAsync(email))
                {
                    _logger.LogError("Failed to send 2FA code for Email: {Email}", account.Email);
                    return new LoginResult { Status = LoginStatus.Failed2FASending };
                }

                return new LoginResult { Status = LoginStatus.Requires2FA, Account = account };
            }

            _loginAttemptTracker.ResetLoginAttempts(clientKey);
            return new LoginResult { Status = LoginStatus.Success, Account = account };
        }

        public async Task AuthorizeAccountAsync(Account account)
        {
            var tokenResponse = _tokenService.GenerateTokenAsync(account);

            await _tokenService.InsertAsync(new Token
            {
                AccountId = tokenResponse.AccountId,
                AccessTokenHash = tokenResponse.AccessToken.HashToken(),
                RefreshToken = tokenResponse.RefreshToken,
                AccessTokenExpiresAt = tokenResponse.AccessTokenExpiresAt,
                RefreshTokenExpiresAt = tokenResponse.RefreshTokenExpiresAt,
                DeviceInfo = UserRequestHelper.GetDeviceDetails()
            });
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