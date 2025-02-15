using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
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
        private IOrderService _orderService;
        private IMapper _mapper;
        private ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, IMapper mapper,ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                request.Order.EmployeeId = (int)HttpContext.Session.GetInt32("Id")!;
                if (request.Order.CustomerId == 0) request.Order.CustomerId = null;
                _logger.LogInformation("EmployeeId: {EmployeeId} placing order", request.Order.EmployeeId);
                int orderId = await _orderService.PlaceOrderAsync(_mapper.Map<Order>(request.Order), _mapper.Map<IEnumerable<OrderDetail>>(request.OrderDetails));

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