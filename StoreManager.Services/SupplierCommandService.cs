using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class SupplierCommandService : ISupplierCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SupplierCommandService> _logger;

        public SupplierCommandService(IUnitOfWork unitOfWork, ILogger<SupplierCommandService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> AddSupplierAsync(Supplier supplier)
        {
            _logger.LogInformation("Adding Supplier in db: {@Supplier}", supplier);

            await _unitOfWork.OpenConnectionAsync();
            try
            {
                int supplierId = (int)await _unitOfWork.SupplierRepository.InsertAsync(supplier);
                _logger.LogInformation("Supplier added successfully with SupplierId: {SupplierId}", supplierId);
                return supplierId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding Supplier {@Supplier}", supplier);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }
    }
}
