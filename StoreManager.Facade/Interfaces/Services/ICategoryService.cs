using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<int> AddCategoryAsync(Category category);
        Task<Category?> GetByNameAsync(string name);
    }
}