using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsyncAsync(Order order, IEnumerable<OrderDetail> orderDetails);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDetail>?> GetOrderDetailsByOrderIdAsync(int orderId);
        Task<OrderDetail?> GetOrderDetailAsync(int orderId, int productId);
        Task DeleteOrderAsync(int orderId);
        Task UpdateOrderAsync(Order order);
    }
}