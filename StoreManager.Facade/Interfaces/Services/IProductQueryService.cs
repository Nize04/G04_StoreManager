using StoreManager.DTO;

public interface IProductQueryService
{
    Task<Product?> GetProductByIdAsync(int id);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
}