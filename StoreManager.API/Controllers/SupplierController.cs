using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;
using StoreManager.Services;

namespace StoreManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeJwt("Manager")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(ISupplierService supplierService, IMapper mapper, ILogger<SupplierController> logger)
        {
            _supplierService = supplierService ?? throw new ArgumentNullException(nameof(supplierService)); ;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("AddSupplier")]
        public async Task<IActionResult> AddSupplier(SupplierModel supplierModel)
        {
            _logger.LogInformation("Received a request to add a new supplier.");

            try
            {
                var supplier = _mapper.Map<Supplier>(supplierModel);
                await _supplierService.AddSupplierAsync(supplier);

                _logger.LogInformation("Supplier added successfully.");
                return Ok("Supplier Added Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a supplier.");
                return StatusCode(500, ex.Message);
            }
        }
    }
}