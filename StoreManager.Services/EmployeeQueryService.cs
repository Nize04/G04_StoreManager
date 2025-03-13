using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Services
{
    public class CategoryQueryService : ICategoryQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryQueryService> _logger;

        public CategoryQueryService(IUnitOfWork unitOfWork, ILogger<CategoryQueryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            _logger.LogInformation("Retrieving category by name: {CategoryName}", name);

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
        }
    }
}
