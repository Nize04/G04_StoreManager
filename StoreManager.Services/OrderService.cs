using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            await _unitOfWork.OpenConnectionAsync();
            try
            {
                return await _unitOfWork.OrderRepository.GetAsync(o => o.IsActive == true);
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<int> PlaceOrderAsync(Order order, IEnumerable<OrderDetail> orderDetails)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (orderDetails == null || orderDetails.Count() == 0) throw new InvalidOperationException("orderDetail is empty");

            _logger.LogInformation("Starting PlacingOrder by EmployeeId {EmoloyeeId}", order.EmployeeId);

            await _unitOfWork.OpenConnectionAsync();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                int orderId = (int)await _unitOfWork.OrderRepository.InsertAsync(order);
                _logger.LogDebug("Order inserted with OrderId {OrderId}", orderId);

                foreach (var orderDetail in orderDetails)
                {
                    await ProcessOrderDetail(orderDetail, orderId);
                }

                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Order successfully placed with OrderId: {OrderId}", orderId);

                return orderId;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();
                _logger.LogError(ex, "Placing Order By EmployeeId {EmployeeId} was unsuccess", order.EmployeeId);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        private async Task ProcessOrderDetail(OrderDetail orderDetail, int orderId)
        {
            if (orderDetail == null)
            {
                _logger.LogWarning("Encountered null OrderDetail. Skipping...");
                throw new ArgumentNullException(nameof(orderDetail), "OrderDetail cannot be null.");
            }

            _logger.LogDebug("Processing OrderDetail for ProductId {ProductId}", orderDetail.ProductId);

            var productQuantity = await _unitOfWork.ProductQuantityRepository.GetByIdAsync(orderDetail.ProductId);
            if (productQuantity == null)
            {
                _logger.LogError("Product with ID {ProductId} not found in the database.", orderDetail.ProductId);
                throw new ArgumentNullException($"Product with ID {orderDetail.ProductId} does not exist.");
            }

            if (productQuantity.Quantity < orderDetail.Quantity)
            {
                _logger.LogWarning("Insufficient stock for ProductId {ProductId}. Requested: {RequestedQuantity}, Available: {AvailableQuantity}",
                    orderDetail.ProductId, orderDetail.Quantity, productQuantity.Quantity);
                throw new InvalidOperationException($"Insufficient stock for ProductId {orderDetail.ProductId}.");
            }

            productQuantity.Quantity -= orderDetail.Quantity;
            _logger.LogDebug("Updated stock for ProductId {ProductId}. Remaining Quantity: {RemainingQuantity}", orderDetail.ProductId, productQuantity.Quantity);

            await _unitOfWork.ProductQuantityRepository.UpdateAsync(productQuantity);

            orderDetail.OrderId = orderId;
            await _unitOfWork.OrderDetailRepository.InsertAsync(orderDetail);

            _logger.LogDebug("OrderDetail for ProductId {ProductId} linked with OrderId {OrderId} inserted successfully.", orderDetail.ProductId, orderId);
        }
    }
}