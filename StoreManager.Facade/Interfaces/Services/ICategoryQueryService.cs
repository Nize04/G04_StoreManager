using StoreManager.DTO;

public interface ICategoryQueryService
{
    Task<Category?> GetByNameAsync(string name);
}