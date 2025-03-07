using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class ProductCommandService : IProductCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductCommandService> _logger;

        public ProductCommandService(IUnitOfWork unitOfWork, ILogger<ProductCommandService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> AddProductAsync(Product product)
        {
            _logger.LogInformation("Adding product with name: {ProductName}", product.Name);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                int productId = (int)await _unitOfWork.ProductRepository.InsertAsync(product);
                _logger.LogInformation("Product added successfully with ID: {ProductId}", productId);
                return productId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding product with name: {ProductName}", product.Name);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task UpdateProductAsync(Product product)
        {
            _logger.LogInformation("Updating product with ID: {ProductId}", product.Id);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                await _unitOfWork.ProductRepository.UpdateAsync(product);
                _logger.LogInformation("Product updated successfully with ID: {ProductId}", product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product with ID: {ProductId}", product.Id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task DeleteProductAsync(int productId)
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", productId);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                await _unitOfWork.ProductRepository.DeleteAsync(productId);
                _logger.LogInformation("Product deleted successfully with ID: {ProductId}", productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting product with ID: {ProductId}", productId);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }
    }
}
