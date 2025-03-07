using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class SupplierTransactionCommandService : ISupplierTransactionCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SupplierTransactionCommandService> _logger;

        public SupplierTransactionCommandService(IUnitOfWork unitOfWork, ILogger<SupplierTransactionCommandService> logger)
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

                int supplierTransactionId = (int)await _unitOfWork.SupplierTransactionRepository.InsertAsync(supplierTransaction);

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
    }
}
