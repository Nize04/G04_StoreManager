using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Models
{
    public class PlaceOrderRequest
    {
        public OrderModel Order { get; set; }
        public IEnumerable<OrderDetailModel> OrderDetails { get; set; }
    }
}
