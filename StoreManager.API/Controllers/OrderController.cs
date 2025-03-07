using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;
using System.Security.Claims;

namespace StoreManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeJwt("Seller")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderCommandService _orderCommandService;
        private readonly IOrderQueryService _orderQueryService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderCommandService orderCommandService,
            IOrderQueryService orderQueryService,
            IMapper mapper,
            ILogger<OrderController> logger)
        {
            _orderCommandService = orderCommandService ?? throw new ArgumentNullException(nameof(orderCommandService));
            _orderQueryService = orderQueryService ?? throw new ArgumentNullException(nameof(orderQueryService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                request.Order.EmployeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                if (request.Order.CustomerId == 0) request.Order.CustomerId = null;
                _logger.LogInformation("EmployeeId: {EmployeeId} placing order", request.Order.EmployeeId);
                int orderId = await _orderCommandService.PlaceOrderAsync(_mapper.Map<Order>(request.Order), _mapper.Map<IEnumerable<OrderDetail>>(request.OrderDetails));

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