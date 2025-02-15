using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;

namespace StoreManager.API.Controllers
{
    [AuthorizeJwt("Admin")] // Role-based authorization
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;
        private readonly IMapper _mapper;

        public RoleController(
            IRoleService roleService,
            ILogger<RoleController> logger,
            IMapper mapper)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRoleToAccount(int accountId, int roleId)
        {
            if (accountId <= 0 || roleId <= 0)
            {
                _logger.LogWarning("AssignRoleToAccount called with invalid accountId {AccountId} or roleId {RoleId}.", accountId, roleId);
                return BadRequest("Invalid account or role ID.");
            }

            try
            {
                _logger.LogInformation("Assigning Role ID {RoleId} to Account ID {AccountId}.", roleId, accountId);
                await _roleService.AssignRoleToAccountAsync(new AccountRole() { AccountId = accountId, RoleId = roleId} );
                _logger.LogInformation("Successfully assigned Role ID {RoleId} to Account ID {AccountId}.", roleId, accountId);

                return Ok($"Role ID {roleId} assigned to Account ID {accountId} successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Failed to assign Role ID {RoleId} to Account ID {AccountId} - Role or Account not found.", roleId, accountId);
                return NotFound("Account or Role not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while assigning Role ID {RoleId} to Account ID {AccountId}.", roleId, accountId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] RoleModel roleModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("AddRole called with invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var role = _mapper.Map<Role>(roleModel);

                _logger.LogInformation("Adding a new role with name: {RoleName}", role.RoleName);
                int roleId = await _roleService.AddRoleAsync(role);

                _logger.LogInformation("Successfully added role with ID: {RoleId}", roleId);
                return Ok("Role added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new role.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}