using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Facade.Interfaces.Trackers;
using StoreManager.Models;

namespace StoreManager.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILoginAttemptTracker _loginAttemptTracker;

        public AccountService(IUnitOfWork unitOfWork,
            ILogger<AccountService> logger,
            ITwoFactorAuthService twoFactorAuthService,
            IBlobStorageService blobStorageService,
            ILoginAttemptTracker loginAttemptTracker)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _twoFactorAuthService = twoFactorAuthService ?? throw new ArgumentNullException(nameof(twoFactorAuthService));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            _loginAttemptTracker = loginAttemptTracker ?? throw new ArgumentNullException(nameof(loginAttemptTracker));
        }

        public async Task<Account?> GetAccountByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve account with ID: {AccountId}", id);

                await _unitOfWork.OpenConnectionAsync();

                Account? account = await _unitOfWork.AccountRepository.GetByIdAsync(id);

                if (account == null)
                {
                    _logger.LogWarning("Account not found for ID: {AccountId}", id);
                }
                else
                {
                    _logger.LogInformation("Account successfully retrieved for ID: {AccountId}", id);
                }

                return account;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving account for ID: {AccountId}", id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve account for Email: {Email}", email);

                await _unitOfWork.OpenConnectionAsync();

                IEnumerable<Account> accounts = await _unitOfWork.AccountRepository.GetAsync(a => a.Email == email && a.IsActive == true);

                if (!accounts.Any())
                {
                    _logger.LogWarning("No account found for Email: {Email}", email);
                    return null;
                }

                Account? account = accounts.FirstOrDefault();

                _logger.LogInformation("Account successfully retrieved for Email: {Email}", email);

                return account;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving account for Email: {Email}", email);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public TwoFAResult Verify2FACode(string email, string code) =>
            _twoFactorAuthService.Verify2FACode(email, code);

        public async Task<LoginResult> ProcessLoginAsync(string email, string password, string clientKey)
        {
            var account = await GetAccountByEmailAsync(email);
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

        private async Task<bool> Send2FACodeAsync(string email)
        {
            return await _twoFactorAuthService.Send2FACodeAsync(email);
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
                _logger.LogError(ex, "Upadating failed EmployeeId: {EmployeeId}", account.Id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }
    }
}