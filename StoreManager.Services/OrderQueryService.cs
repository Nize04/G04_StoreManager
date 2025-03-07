using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;

namespace StoreManager.Services
{
    public class OrderQueryService : IOrderQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderQueryService> _logger;

        public OrderQueryService(IUnitOfWork unitOfWork, ILogger<OrderQueryService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            _logger.LogInformation("Starting GetOrderByIdAsync for OrderId {OrderId}", id);
            await _unitOfWork.OpenConnectionAsync();
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
                _logger.LogInformation("Order with OrderId {OrderId} retrieved successfully", id);
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving Order with OrderId {OrderId}", id);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<OrderDetail?> GetOrderDetailAsync(int orderId, int productId)
        {
            _logger.LogInformation("Starting GetOrderDetailAsync for OrderId {OrderId} and ProductId {ProductId}", orderId, productId);
            await _unitOfWork.OpenConnectionAsync();
            try
            {
                var orderDetail = await _unitOfWork.OrderDetailRepository.GetByIdAsync(orderId, productId);
                _logger.LogInformation("OrderDetail for OrderId {OrderId} and ProductId {ProductId} retrieved successfully", orderId, productId);
                return orderDetail;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving OrderDetail for OrderId {OrderId} and ProductId {ProductId}", orderId, productId);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<IEnumerable<OrderDetail>?> GetOrderDetailsByOrderIdAsync(int orderId)
        {
            _logger.LogInformation("Starting GetOrderDetailsByOrderIdAsync for OrderId {OrderId}", orderId);
            await _unitOfWork.OpenConnectionAsync();
            try
            {
                var orderDetails = await _unitOfWork.OrderDetailRepository.GetAsync(od => od.OrderId == orderId && od.IsActive == true);
                _logger.LogInformation("OrderDetails for OrderId {OrderId} retrieved successfully", orderId);
                return orderDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving OrderDetails for OrderId {OrderId}", orderId);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }
    }
}
