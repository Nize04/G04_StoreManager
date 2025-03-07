using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class ProductQueryService : IProductQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductQueryService> _logger;

        public ProductQueryService(IUnitOfWork unitOfWork, ILogger<ProductQueryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving product by ID: {ProductId}", id);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("No product found with ID: {ProductId}", id);
                }
                else
                {
                    _logger.LogInformation("Product retrieved successfully with ID: {ProductId}", id);
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving product with ID: {ProductId}", id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            _logger.LogInformation("Retrieving products by category ID: {CategoryId}", categoryId);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                var products = await _unitOfWork.ProductRepository.GetAsync(p => p.CategoryId == categoryId);

                if (!products.Any())
                {
                    _logger.LogWarning("No products found for category ID: {CategoryId}", categoryId);
                }
                else
                {
                    _logger.LogInformation("Products retrieved successfully for category ID: {CategoryId}", categoryId);
                }

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products for category ID: {CategoryId}", categoryId);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }
    }
}
