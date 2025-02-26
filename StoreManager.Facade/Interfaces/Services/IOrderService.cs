using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(Order order, IEnumerable<OrderDetail> orderDetails);

        Task<IEnumerable<Order>> GetOrdersAsync();
    }
}