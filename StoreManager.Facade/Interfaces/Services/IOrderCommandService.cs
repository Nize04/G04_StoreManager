using StoreManager.DTO;

public interface IOrderCommandService
{
    Task<int> PlaceOrderAsync(Order order, IEnumerable<OrderDetail> orderDetails);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(int orderId);
}