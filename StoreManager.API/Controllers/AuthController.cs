using Microsoft.AspNetCore.Mvc;
using MyAttributes;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;

namespace StoreManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountCommandService _accountCommandService;
        private readonly IAccountQueryService _accountQueryService;
        private readonly ITokenService _tokenService;
        private readonly ISessionService _sessionService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAccountCommandService accountCommandService,
            IAccountQueryService accountQueryService,
            ITokenService tokenService,
            ISessionService sessionService,
            ILogger<AuthController> logger)
        {
            _accountCommandService = accountCommandService ?? throw new ArgumentNullException(nameof(accountCommandService));
            _accountQueryService = accountQueryService ?? throw new ArgumentNullException(nameof(accountQueryService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                _logger.LogWarning("Login attempt failed. Invalid model received.");
                return BadRequest(new { message = "Invalid login credentials." });
            }

            var clientKey = $"{Request.HttpContext.Connection.RemoteIpAddress}:{model.Email}";

            try
            {
                var result = await _accountCommandService.ProcessLoginAsync(model.Email, model.Password, clientKey);

                switch (result.Status)
                {
                    case LoginStatus.Success:
                        await _accountCommandService.AuthorizeAccountAsync(result.Account);
                        return Ok(new { message = "Login Successful" });

                    case LoginStatus.Requires2FA:
                        _sessionService.CustomSession(new Dictionary<string, object> { { "Email", result.Account.Email } });
                        return BadRequest(new { message = "Please enter the 2FA code sent to your email." });

                    case LoginStatus.LockedOut:
                        return StatusCode(429, new { message = "Too many failed login attempts. Please try again later." });

                    case LoginStatus.InvalidCredentials:
                    default:
                        return Unauthorized(new { message = "Invalid credentials." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for Email: {Email}", model.Email);
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
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
                string email = _sessionService.GetString("Email");
                var result = _accountCommandService.Verify2FACode(email, code);

                switch (result)
                {
                    case TwoFAResult.LockedOut:
                        _logger.LogWarning("2FA verification locked out for Email: {Email}", email);
                        return Problem("Too many failed 2FA attempts. Please try again later.", statusCode: 429);
                    case TwoFAResult.InvalidCode:
                        _logger.LogWarning("2FA verification failed for Email: {Email}", email);
                        return Unauthorized("Invalid 2FA code.");
                    case TwoFAResult.Success:
                        var account = await _accountQueryService.GetAccountByEmailAsync(email);
                        await _accountCommandService.AuthorizeAccountAsync(account);
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

        [HttpPost]
        [AuthorizeJwt]
        [Route("logout")]
        public async Task<IActionResult> LogOut()
        {
            _sessionService.Clear();

            var jwtToken = HttpContext.Request.Cookies["jwtToken"];
            if (!string.IsNullOrEmpty(jwtToken))
            {
                await _tokenService.RevokeTokenAsync(jwtToken);
            }

            HttpContext.Response.Cookies.Delete("refreshtoken");
            HttpContext.Response.Cookies.Delete("jwtToken");

            return Ok("LogOut successful");
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return BadRequest("Refresh token is missing.");
            }

            try
            {
                var newToken = await _tokenService.RefreshToken(refreshToken);
                return Ok(new { newToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token.");
                return StatusCode(500, "An error occurred while refreshing the token.");
            }
        }
    }
}
