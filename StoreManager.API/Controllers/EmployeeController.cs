using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Models;

namespace StoreManager.API.Controllers
{
    [AuthorizeJwt("Manager")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeCommandService _employeeCommandService;
        private readonly IEmployeeQueryService _employeeQueryService;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IMapper _mapper;

        public EmployeeController(
            IEmployeeCommandService employeeCommandService,
            IEmployeeQueryService employeeQueryService,
            ILogger<EmployeeController> logger,
            IMapper mapper)
        {
            _employeeCommandService = employeeCommandService ?? throw new ArgumentNullException(nameof(employeeCommandService));
            _employeeQueryService = employeeQueryService ?? throw new ArgumentNullException(nameof(employeeQueryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeModel employeeModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("AddEmployee called with invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Adding new employee. FirstName: {FirstName}, LastName: {LastName}",
                    employeeModel.FirstName, employeeModel.LastName);

                var employee = _mapper.Map<Employee>(employeeModel);
                int employeeId = await _employeeCommandService.AddEmployeeAsync(employee);

                _logger.LogInformation("Successfully added employee with ID {EmployeeId}", employeeId);
                return CreatedAtAction(nameof(GetEmployeeById), new { employeeId }, "Employee added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding an employee. FirstName: {FirstName}, LastName: {LastName}",
                    employeeModel.FirstName, employeeModel.LastName);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetEmployeeById/{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            if (employeeId <= 0)
            {
                _logger.LogWarning("GetEmployeeById called with invalid employeeId: {EmployeeId}", employeeId);
                return BadRequest("Invalid Employee ID.");
            }

            try
            {
                _logger.LogInformation("Retrieving employee with ID {EmployeeId}", employeeId);

                var employee = await _employeeQueryService.GetEmployeeByIdAsync(employeeId);
                if (employee == null)
                {
                    _logger.LogWarning("Employee not found for ID {EmployeeId}", employeeId);
                    return NotFound($"Employee with ID {employeeId} not found.");
                }

                var employeeModel = _mapper.Map<EmployeeModel>(employee);
                _logger.LogInformation("Successfully retrieved employee with ID {EmployeeId}", employeeId);
                return Ok(employeeModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving employee with ID {EmployeeId}", employeeId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}