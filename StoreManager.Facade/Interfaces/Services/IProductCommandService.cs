using StoreManager.DTO;

public interface IProductCommandService
{
    Task<int> AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int productId);
}