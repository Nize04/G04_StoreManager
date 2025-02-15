using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Tests.ServiceTests
{
    [Collection("Database Tests")]
    public class OrderServiceTest : TestBase
    {
        private IOrderService _orderService;

        public OrderServiceTest(IUnitOfWork unitOfWork, DatabaseFixture fixture, IOrderService orderService) : base(unitOfWork, fixture)
        {
            _orderService = orderService;
        }

        [Fact]
        public async Task OrderPlace()
        {
            Order order = new Order() { CustomerId = 1, EmployeeId = 2, Description = "We love Rich Customers" };
            OrderDetail[] orderDetails = new[] { new OrderDetail() { ProductId = 1, Quantity = 10, UnitPrice = (decimal)749.99 },
            new OrderDetail() { ProductId = 2, Quantity = 11, UnitPrice = (decimal)999.99}};

            _unitOfWork.OpenConnectionAsync();
            int orderId = (int)await _orderService.PlaceOrderAsync(order, orderDetails);
            Order? insertedOrder = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

            Assert.NotNull(insertedOrder);

            Assert.Equal(insertedOrder.Description, order.Description);

            foreach(OrderDetail orderDetail in orderDetails)
            {
                IEnumerable<OrderDetail> insertedOrderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == orderId && od.ProductId == orderDetail.ProductId);
                OrderDetail insertedOrderDetail = insertedOrderDetails.FirstOrDefault();
                Assert.NotNull(insertedOrderDetail);
                Assert.Equal(insertedOrderDetail.UnitPrice, orderDetail.UnitPrice);
            }
            ProductsQuantity firstOrder = await _unitOfWork.ProductQuantityRepository.GetByIdAsync(1)!;
            ProductsQuantity secondeOrder = await _unitOfWork.ProductQuantityRepository.GetByIdAsync(2)!;

            Assert.True(firstOrder.Quantity >= 0);
            Assert.True(secondeOrder.Quantity >= 0);

        }
    }
}
