using StoreManager.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(Order order, IEnumerable<OrderDetail> orderDetails);

        Task<IEnumerable<Order>> GetOrdersAsync();
    }
}
