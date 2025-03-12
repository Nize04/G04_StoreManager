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
    public class AccountController : ControllerBase
    {
        private readonly IAccountCommandService _accountCommandService;
        private readonly IAccountQueryService _accountQueryService;
        private readonly IRoleService _roleService;
        private readonly ISessionService _sessionService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAccountCommandService accountCommandService,
            IAccountQueryService accountQueryService,
            IAccountImageService accountImageService,
            IRoleService roleService,
            IMapper mapper,
            ILogger<AccountController> logger,
            ISessionService sessionService)
        {
            _accountCommandService = accountCommandService ?? throw new ArgumentNullException(nameof(accountCommandService));
            _accountQueryService = accountQueryService ?? throw new ArgumentNullException(nameof(accountQueryService));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAccount(RegisterModel accountModel)
        {
            if (accountModel == null)
            {
                return BadRequest("Invalid registration details.");
            }

            try
            {
                _logger.LogInformation("Registration started for EmployeeId: {EmployeeId}", accountModel.Id);
                await _accountCommandService.RegisterAsync(_mapper.Map<Account>(accountModel));
                _logger.LogInformation("Registration successful for EmployeeId: {EmployeeId}", accountModel.Id);

                return Ok("Account registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for EmployeeId: {EmployeeId}", accountModel.Id);
                return StatusCode(500, "An error occurred during registration.");
            }
        }

        [HttpGet("GetAccountById")]
        [AuthorizeJwt("Admin")]
        public async Task<IActionResult> GetAccountById(int accountId)
        {
            try
            {
                var account = await _accountQueryService.GetAccountByIdAsync(accountId);
                if (account == null)
                {
                    _logger.LogWarning("⚠️ Account not found for AccountId: {AccountId}", accountId);
                    return NotFound("Account not found.");
                }

                var roleModels = _mapper.Map<IEnumerable<RoleModel>>(await _roleService.GetRolesByAccountIdAsync(accountId));
                var model = _mapper.Map<AccountModel>(account);
                model.Roles = roleModels;

                _logger.LogInformation("✅ Successfully retrieved account details for AccountId: {AccountId}", accountId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving account for AccountId: {AccountId}", accountId);
                return StatusCode(500, "An error occurred while retrieving the account.");
            }
        }

        [HttpPost("set-2fa")]
        [AuthorizeJwt]
        public async Task<IActionResult> Set2FA(string password)
        {
            try
            {
                int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                await _accountCommandService.EnableTwoFactorAuthAsync(accountId, password);

                return Ok("2FA has been successfully enabled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enabling 2FA for AccountId: {AccountId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "An error occurred while enabling 2FA.");
            }
        }

        [HttpPost("ChangePassword")]
        [AuthorizeJwt]
        public async Task<IActionResult> PasswordChange(string oldPassword, string newPassword)
        {
            try
            {
                int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                await _accountCommandService.ChangePasswordAsync(accountId, oldPassword, newPassword);

                _logger.LogInformation("✅ Password successfully changed for AccountId: {AccountId}", accountId);
                return Ok("Password changed successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("⚠️ Unauthorized password change attempt for AccountId: {AccountId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error changing password for AccountId: {AccountId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, "An error occurred while changing the password.");
            }
        }

        private string GetSessionEmail()
        {
            var email = _sessionService.GetString("Email");
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new UnauthorizedAccessException("Session expired or email is missing.");
            }
            return email;
        }
    }
}
