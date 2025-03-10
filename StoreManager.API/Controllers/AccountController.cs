using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;

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
            var account = await _accountQueryService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return NotFound("Account not found.");
            }

            var roleModels = _mapper.Map<IEnumerable<RoleModel>>(await _roleService.GetRolesByAccountIdAsync(accountId));
            AccountModel model = new AccountModel()
            {
                Email = account.Email,
                Roles = roleModels
            };

            return Ok(model);
        }

        [HttpPost("set-2fa")]
        [AuthorizeJwt]
        public async Task<IActionResult> Set2FA(string password)
        {
            string email = GetSessionEmail();
            var account = await _accountQueryService.GetAccountByEmailAsync(email);

            if (account!.Requires2FA)
            {
                return BadRequest("This account already has 2FA enabled.");
            }

            if (!PasswordHelper.ValidatePassword(password, account.Password, account.Salt))
            {
                return Unauthorized("Invalid password.");
            }

            account.Requires2FA = true;
            await _accountCommandService.UpdateAccount(account);

            _logger.LogInformation("2FA successfully enabled for Email: {Email}", email);
            return Ok("2FA has been successfully enabled.");
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
