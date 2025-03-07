using StoreManager.DTO;

public interface IOrderQueryService
{
    Task<Order?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDetail>?> GetOrderDetailsByOrderIdAsync(int orderId);
    Task<OrderDetail?> GetOrderDetailAsync(int orderId, int productId);
}