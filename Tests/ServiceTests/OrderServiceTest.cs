using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Tests.ServiceTests
{
    [Collection("Database Tests")]
    public class OrderServiceTest : TestBase
    {
        private IOrderService _orderService;

        public OrderServiceTest(DatabaseFixture fixture, IOrderService orderService) : base(fixture)
        {
            _orderService = orderService;
        }

        [Fact]
        public async Task OrderPlace()
        {
            Order order = new Order() { CustomerId = 1, EmployeeId = 2, Description = "We love Rich Customers" };
            OrderDetail[] orderDetails = new[] { new OrderDetail() { ProductId = 1, Quantity = 10, UnitPrice = (decimal)749.99 },
            new OrderDetail() { ProductId = 2, Quantity = 11, UnitPrice = (decimal)999.99}};

            int orderId = await _orderService.PlaceOrderAsyncAsync(order, orderDetails);
            Order? insertedOrder = await _orderService.GetOrderByIdAsync(orderId);

            Assert.NotNull(insertedOrder);

            Assert.Equal(insertedOrder.Description, order.Description);

            foreach (OrderDetail orderDetail in orderDetails)
            {
                OrderDetail? insertedOrderDetail = await _orderService.GetOrderDetailAsync(orderDetail.OrderId, orderDetail.ProductId);
                Assert.NotNull(insertedOrderDetail);
                Assert.Equal(insertedOrderDetail.UnitPrice, orderDetail.UnitPrice);
            }
        }

        [Fact]
        public async Task Update()
        {
            Order? order = await _orderService.GetOrderByIdAsync(1);

            Assert.NotNull(order);
            order.Description = "Updated Description";

            await _orderService.UpdateOrderAsync(order);

            Order updatedOrder = await _orderService.GetOrderByIdAsync(order.Id);

            Assert.True(order.Description == updatedOrder!.Description);
        }

        [Fact]
        public async Task Delete()
        {
            Order? order = await _orderService.GetOrderByIdAsync(2);

            Assert.NotNull(order);

            await _orderService.DeleteOrderAsync(order.Id);

            Assert.Null(await _orderService.GetOrderByIdAsync(order.Id));

            Assert.Empty(await _orderService.GetOrderDetailsByOrderIdAsync(order.Id));
        }

        [Fact]
        public async Task OrderPlace_InvalidProductsQuantity_ShouldFail()
        {
            Order order = new Order() { CustomerId = 1, EmployeeId = 2, Description = null };
            OrderDetail[] orderDetails = new[]
            {
                new OrderDetail() { ProductId = 1, Quantity = -5, UnitPrice = 749.99m },
                new OrderDetail() { ProductId = 2, Quantity = -5, UnitPrice = 999.99m }
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _orderService.PlaceOrderAsyncAsync(order, orderDetails));
        }

        [Fact]
        public async Task OrderPlace_EmptyOrderDetails_ShouldFail()
        {
            Order order = new Order() { CustomerId = 1, EmployeeId = 2, Description = "We love Rich Customers" };
            OrderDetail[] orderDetails = Array.Empty<OrderDetail>();

            await Assert.ThrowsAsync<ArgumentException>(() => _orderService.PlaceOrderAsyncAsync(order, orderDetails));
        }
    }
}