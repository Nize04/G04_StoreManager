using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;

namespace StoreManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeJwt("Seller")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ISessionService _sessionService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService,
            IMapper mapper,
            ISessionService sessionService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                request.Order.EmployeeId = (int)_sessionService.GetInt32("Id")!;

                if (request.Order.CustomerId == 0) request.Order.CustomerId = null;
                _logger.LogInformation("EmployeeId: {EmployeeId} placing order", request.Order.EmployeeId);
                int orderId = await _orderService.PlaceOrderAsyncAsync(_mapper.Map<Order>(request.Order), _mapper.Map<IEnumerable<OrderDetail>>(request.OrderDetails));

                _logger.LogInformation("OrderId: {OrderId} placed successfully by EmployeeId: {EmployeeId}", orderId, request.Order.EmployeeId);
                return Ok("Order Placed Succesfully");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Placing order was unccussesfull");
                return StatusCode(500, ex.Message);
            }
        }
    }
}