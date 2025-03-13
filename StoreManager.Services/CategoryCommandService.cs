using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Services
{
    public class CategoryCommandService : ICategoryCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryCommandService> _logger;

        public CategoryCommandService(IUnitOfWork unitOfWork, ILogger<CategoryCommandService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<int> AddCategoryAsync(Category category)
        {
            _logger.LogInformation("Adding category with name: {CategoryName}", category.Name);

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
        }
    }
}
