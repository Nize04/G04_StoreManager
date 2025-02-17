using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Extensions;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;
using System.Data;

namespace StoreManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly ISessionService _sessionService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAccountService accountService,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<AccountController> logger,
            ISessionService sessionService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                _logger.LogWarning("Login attempt failed. Invalid model received.");
                return BadRequest("Invalid login credentials.");
            }

            var clientKey = $"{Request.HttpContext.Connection.RemoteIpAddress}:{model.Email}";

            try
            {
                var result = await _accountService.ProcessLoginAsync(model.Email, model.Password, clientKey);

                switch (result.Status)
                {
                    case LoginStatus.Success:

                        await Authorize(result.Account);
                        return Ok("Login Successful");

                    case LoginStatus.Requires2FA:
                        _sessionService.CustomeSession(new Dictionary<string, object>() { { "Email", result.Account.Email } });
                        return BadRequest("Please enter the 2FA code sent to your email.");

                    case LoginStatus.LockedOut:
                        return StatusCode(429, "Too many failed login attempts. Please try again later.");

                    case LoginStatus.InvalidCredentials:
                    default:
                        return Unauthorized("Invalid credentials.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for Email: {Email}", model.Email);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [AuthorizeJwt]
        [Route("LogOut")]
        public async Task<IActionResult> LogOut()
        {
            _sessionService.Clear();

            var jwtToken = HttpContext.Request.Cookies["jwtToken"];
            if (!string.IsNullOrEmpty(jwtToken))
            {
                await _tokenService.RevokeTokenAsync(jwtToken);
            }

            HttpContext.Response.Cookies.Delete("jwtToken");

            return Ok("LogOut successful");
        }

        [HttpPost]
        [Route("verify-2fa")]
        public async Task<IActionResult> Verify2FACode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Invalid code.");
            }

            try
            {
                string email = GetSessionEmail();
                var result = _accountService.Verify2FACodeAsync(email, code);

                switch (result)
                {
                    case TwoFAResult.LockedOut:
                        _logger.LogWarning("2FA verification locked out for Email: {Email}", email);
                        return Problem("Too many failed 2FA attempts. Please try again later.", statusCode: 429);
                    case TwoFAResult.InvalidCode:
                        _logger.LogWarning("2FA verification failed for Email: {Email}", email);
                        return Unauthorized("Invalid 2FA code.");
                    case TwoFAResult.Success:
                        var account = await _accountService.GetAccountByEmailAsync(email);
                        await Authorize(account!);
                        return Ok("2FA verification successful.");
                    default:
                        return StatusCode(500, "Unknown error during 2FA verification.");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during 2FA verification.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetAccountById")]
        [AuthorizeJwt("Admin")]
        public async Task<IActionResult> GetAccountById(int accountId)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return NotFound("Account not found.");
            }
            return Ok(_mapper.Map<LoginModel>(account));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAccount(RegisterModel accountModel)
        {
            if (accountModel == null)
            {
                return BadRequest("Invalid registration details.");
            }

            try
            {
                _logger.LogInformation("Registration started for EmployeeId: {EmployeeId}", accountModel.Id);
                await _accountService.RegisterAsync(_mapper.Map<Account>(accountModel));
                _logger.LogInformation("Registration successful for EmployeeId: {EmployeeId}", accountModel.Id);

                return Ok("Account registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for EmployeeId: {EmployeeId}", accountModel.Id);
                return StatusCode(500, "An error occurred during registration.");
            }
        }

        [HttpPost("set-2fa")]
        [AuthorizeJwt]
        public async Task<IActionResult> Set2FA(string password)
        {
            string email = GetSessionEmail();
            var account = await _accountService.GetAccountByEmailAsync(email);

            if (account!.Requires2FA)
            {
                return BadRequest("This account already has 2FA enabled.");
            }

            if (!PasswordHelper.ValidatePassword(password, account.Password, account.Salt))
            {
                return Unauthorized("Invalid password.");
            }

            account.Requires2FA = true;
            await _accountService.UpdateAccount(account);

            _logger.LogInformation("2FA successfully enabled for Email: {Email}", email);
            return Ok("2FA has been successfully enabled.");
        }

        private async Task Authorize(Account account)
        {
            var tokenResponse = _tokenService.GenerateToken(account);

            await _tokenService.InsertAsync(new Token
            {
                AccountId = tokenResponse.AccountId,
                TokenHash = tokenResponse.Token,
                ExpiresAt = tokenResponse.ExpiresAt,
                DeviceInfo = UserRequestHelper.GetDeviceDetails()
            });

            _sessionService.CustomeSession(new Dictionary<string, object>
            {
                { "Id", account.Id },
                { "Email", account.Email }
            });
        }

        private string GetSessionEmail()
        {
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new UnauthorizedAccessException("Session expired or email is missing.");
            }
            return email;
        }
    }
}