using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            _logger.LogInformation("Adding category with name: {CategoryName}", category.Name);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                int categoryId = (int)await _unitOfWork.CategoryRepository.InsertAsync(category);
                _logger.LogInformation("Category added successfully with ID: {CategoryId}", categoryId);
                return categoryId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding category with name: {CategoryName}", category.Name);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            _logger.LogInformation("Retrieving category by name: {CategoryName}", name);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                IEnumerable<Category> categories = await _unitOfWork.CategoryRepository.GetAsync(c => c.Name == name);
                Category? category = categories.FirstOrDefault();

                if (category == null)
                {
                    _logger.LogWarning("No category found with name: {CategoryName}", name);
                }
                else
                {
                    _logger.LogInformation("Category retrieved successfully with name: {CategoryName}", name);
                }

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with name: {CategoryName}", name);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }
    }
}