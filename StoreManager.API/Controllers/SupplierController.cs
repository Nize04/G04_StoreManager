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
    [AuthorizeJwt("Manager")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierCommandService _supplierCommandService;
        private readonly ISupplierQueryService _supplierQueryService;
        private readonly IMapper _mapper;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(
            ISupplierCommandService supplierCommanService, 
            ISupplierQueryService supplierQueryService,
            IMapper mapper, 
            ILogger<SupplierController> logger)
        {
            _supplierCommandService = supplierCommanService ?? throw new ArgumentNullException(nameof(supplierCommanService));
            _supplierQueryService = supplierQueryService ?? throw new ArgumentNullException(nameof(supplierQueryService));
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
                await _supplierCommandService.AddSupplierAsync(supplier);

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