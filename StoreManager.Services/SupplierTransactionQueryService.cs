using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class SupplierTransactionQueryService : ISupplierTransactionQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SupplierTransactionQueryService> _logger;

        public SupplierTransactionQueryService(IUnitOfWork unitOfWork, ILogger<SupplierTransactionQueryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SupplierTransaction?> GetTransactionByIdAsync(int id)
        {
            _logger.LogInformation("Attempting to retrieve SupplierTransaction with ID: {SupplierTransactionId}", id);

            await _unitOfWork.OpenConnectionAsync();
            try
            {
                var transaction = await _unitOfWork.SupplierTransactionRepository.GetByIdAsync(id);

                if (transaction == null) _logger.LogWarning("SupplierTransaction with ID {TransactionId} not found.", id);

                else _logger.LogInformation("Successfully retrieved SupplierTransaction with ID: {TransactionId}", id);

                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching SupplierTransaction with ID {TransactionId}", id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
                _logger.LogInformation("Connection closed for fetching SupplierTransaction with ID: {TransactionId}", id);
            }
        }

        public async Task<IEnumerable<SupplierTransactionDetail>?> GetTransactionDetailsAsync(int supplierTransactionId)
        {
            _logger.LogInformation("Fetching details for SupplierTransactionId: {TransactionId}", supplierTransactionId);

            await _unitOfWork.OpenConnectionAsync();
            try
            {
                IEnumerable<SupplierTransactionDetail>? transactionDetails = await _unitOfWork.SupplierTransactionDetailRepository.GetAsync(od => od.SupplierTransactionId == supplierTransactionId);

                if (transactionDetails == null || !transactionDetails.Any()) _logger.LogWarning("No details found for SupplierTransactionId: {TransactionId}", supplierTransactionId);

                else _logger.LogInformation("Successfully retrieved {DetailCount} details for SupplierTransactionId: {TransactionId}", transactionDetails.Count(), supplierTransactionId);

                return transactionDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching details for SupplierTransactionId: {TransactionId}", supplierTransactionId);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
                _logger.LogInformation("Connection closed for fetching details of SupplierTransactionId: {TransactionId}", supplierTransactionId);
            }
        }
    }
}
