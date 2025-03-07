﻿using AutoMapper;
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
    [AuthorizeJwt("Manager")]
    public class SupplierTransactionController : ControllerBase
    {
        private readonly ISupplierTransactionCommandService _supplierTransactionCommandService;
        private readonly ISupplierTransactionQueryService _supplierTransactionQueryService;
        private readonly ISessionService _sessionService;
        private readonly IMapper _mapper;
        private readonly ILogger<SupplierTransactionController> _logger;

        public SupplierTransactionController(
            ISupplierTransactionCommandService supplierTransactionService,
            ISupplierTransactionQueryService supplierTransactionQueryService,
            IMapper mapper,
            ISessionService sessionService,
            ILogger<SupplierTransactionController> logger)
        {
            _supplierTransactionCommandService = supplierTransactionService ?? throw new ArgumentNullException(nameof(supplierTransactionService));
            _supplierTransactionQueryService = supplierTransactionQueryService ?? throw new ArgumentNullException(nameof(supplierTransactionQueryService));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("MakeTransaction")]
        public async Task<IActionResult> MakeSupplierTransaction([FromBody] MakeSupplierTransactionRequest makeSupplierTransactionRequest)
        {
            _logger.LogInformation("Received a request to make a supplier transaction.");

            try
            {
                SupplierTransaction supplierTransaction = _mapper.Map<SupplierTransaction>(
                    makeSupplierTransactionRequest.InputSupplierTransactionModel);

                supplierTransaction.EmployeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                _logger.LogInformation("Creating supplier transaction by EmployeeId: {EmployeeId}", supplierTransaction.EmployeeId);

                await _supplierTransactionCommandService.CreateTransactionAsync(supplierTransaction,
                    _mapper.Map<IEnumerable<SupplierTransactionDetail>>(makeSupplierTransactionRequest.SupplierTransactionDetails));

                _logger.LogInformation("Supplier transaction created successfully.");
                return Ok("Supplier Transaction Created Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a supplier transaction.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetTransactionById")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            _logger.LogInformation("Fetching supplier transaction by ID: {TransactionId}", id);

            try
            {
                SupplierTransaction? supplierTransaction = await _supplierTransactionQueryService.GetTransactionByIdAsync(id);

                if (supplierTransaction == null)
                {
                    _logger.LogWarning("Supplier transaction with ID {TransactionId} does not exist.", id);
                    return NotFound("Supplier transaction with this ID doesn't exist.");
                }

                _logger.LogInformation("Supplier transaction found, fetching transaction details.");
                IEnumerable<SupplierTransactionDetail> sTransactionDetails =
                    await _supplierTransactionQueryService.GetTransactionDetailsAsync(supplierTransaction.Id);

                var result = new GetSupplierTransactionModel()
                {
                    SupplierTransaction = supplierTransaction,
                    SupplierTransactionDetails = sTransactionDetails!
                };

                _logger.LogInformation("Successfully retrieved supplier transaction.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving supplier transaction with ID {TransactionId}.", id);
                return StatusCode(500, ex.Message);
            }
        }
    }
}