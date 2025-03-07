using StoreManager.DTO;

public interface ICategoryCommandService
{
    Task<int> AddCategoryAsync(Category category);
}
