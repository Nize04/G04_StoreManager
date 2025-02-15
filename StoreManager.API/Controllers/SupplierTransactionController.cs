
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
    public class SupplierTransactionController : ControllerBase
    {
        private ISupplierTransactionService _supplierTransactionService;
        private IMapper _mapper;

        public SupplierTransactionController(ISupplierTransactionService supplierTransactionService, IMapper mapper)
        {
            _supplierTransactionService = supplierTransactionService;
            _mapper = mapper;
        }

        [HttpPost("MakeTransaction")]
        public async Task<IActionResult> MakeSupplierTransaction([FromBody] MakeSupplierTransactionRequest makeSupplierTransactionRequest)
        {
            try
            {
                SupplierTransaction supplierTransaction = _mapper.Map<SupplierTransaction>
                (makeSupplierTransactionRequest.InputSupplierTransactionModel);

                supplierTransaction.EmployeeId = (int)HttpContext.Session.GetInt32("Id")!;

                await _supplierTransactionService.CreateTransactionAsync(supplierTransaction,
                    _mapper.Map<IEnumerable<SupplierTransactionDetail>>(makeSupplierTransactionRequest.SupplierTransactionDetails));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok("Supplier Transaction Created Succesfully");
        }

        [HttpGet("GetTransactionById")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            try
            {
                SupplierTransaction? supplierTransaction = await _supplierTransactionService.GetTransactionByIdAsync(id);
                if (supplierTransaction == null)
                {
                    return NotFound("supplierTransaction with this id doesnt exists");
                }
                IEnumerable<SupplierTransactionDetail> sTransactionDetails = await _supplierTransactionService.GetTransactionDetailsAsync(supplierTransaction.Id);
                GetSupplierTransactionModel result = new GetSupplierTransactionModel()
                {
                    SupplierTransaction = supplierTransaction,
                    SupplierTransactionDetails = sTransactionDetails
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}