using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class SupplierTransactionService : ISupplierTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SupplierTransactionService> _logger;

        public SupplierTransactionService(IUnitOfWork unitOfWork, ILogger<SupplierTransactionService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateTransactionAsync(SupplierTransaction supplierTransaction, IEnumerable<SupplierTransactionDetail> supplierTransactionDetails)
        {
            _logger.LogInformation("Starting creation of transaction for SupplierTransaction {SupplierTransaction}", supplierTransaction);

            await _unitOfWork.OpenConnectionAsync();
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                _logger.LogInformation("Starting creation of transaction for SupplierTransaction {SupplierTransaction}", supplierTransaction);

                int supplierTransactionId = (int)await _unitOfWork.SupplierTransactionRepository.
                    InsertAsync(supplierTransaction);

                _logger.LogInformation("Inserted SupplierTransaction with SupplierTransactionId {SupplierTransactionId}", supplierTransactionId);

                foreach (SupplierTransactionDetail sTranDetl in supplierTransactionDetails)
                {
                    _logger.LogInformation("Processing SupplierTransactionDetail for ProductId {ProductId} and Quantity {Quantity}", sTranDetl.ProductId, sTranDetl.Quantity);

                    sTranDetl.SupplierTransactionId = supplierTransactionId;
                    ProductsQuantity? pQuantity = await _unitOfWork.ProductQuantityRepository.GetByIdAsync(sTranDetl.ProductId);
                    if (pQuantity == null)
                    {
                        _logger.LogError("Product with ProductId {ProductId} not found. Unable to process transaction detail.", sTranDetl.ProductId);
                        throw new ArgumentNullException($"{sTranDetl.ProductId} product with this id is not exists");
                    }
                    pQuantity.Quantity += sTranDetl.Quantity;
                    await _unitOfWork.ProductQuantityRepository.UpdateAsync(pQuantity);
                    _logger.LogInformation("Update ProductQuantity for ProductId {ProductId}", sTranDetl.ProductId);

                    await _unitOfWork.SupplierTransactionDetailRepository.InsertAsync(sTranDetl);
                    _logger.LogInformation("Inserted SupplierTransactionDetail for ProductId {ProductId}", sTranDetl.ProductId);
                }

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Transaction committed successfully for SupplierTransactionId {SupplierTransactionId}", supplierTransactionId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                _logger.LogError(ex, "Error occurred while creating transaction for SupplierTransactionId {SupplierTransactionId}", supplierTransaction.Id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
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