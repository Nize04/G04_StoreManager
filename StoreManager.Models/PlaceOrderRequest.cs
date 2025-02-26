namespace StoreManager.Models
{
    public class PlaceOrderRequest
    {
        public OrderModel Order { get; set; } = null!;
        public IEnumerable<OrderDetailModel> OrderDetails { get; set; } = null!;
    }
}