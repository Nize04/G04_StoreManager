using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Services
{
    public class AccountQueryService : IAccountQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountQueryService> _logger;

        public AccountQueryService(IUnitOfWork unitOfWork, ILogger<AccountQueryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve account for Email: {Email}", email);

                await _unitOfWork.OpenConnectionAsync();
                Account? account = await _unitOfWork.AccountRepository.GetByEmailAsync(email);
                if (account != null)
                {
                    _logger.LogInformation("Account successfully retrieved for Email: {Email}", email);
                }
                else throw new ArgumentException("account with this email not found");

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
    }
}
