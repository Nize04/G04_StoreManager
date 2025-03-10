using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Extensions;
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
        private readonly IAccountImageService _accountImageService;
        private readonly ISessionService _sessionService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAccountCommandService accountCommandService,
            IAccountQueryService accountQueryService,
            IAccountImageService accountImageService,
            IMapper mapper,
            ILogger<AccountController> logger,
            ISessionService sessionService)
        {
            _accountCommandService = accountCommandService ?? throw new ArgumentNullException(nameof(accountCommandService));
            _accountQueryService = accountQueryService ?? throw new ArgumentNullException(nameof(accountQueryService));
            _accountImageService = accountImageService ?? throw new ArgumentNullException(nameof(accountImageService));
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
            return Ok(new {account.Email});
        }

        [HttpPost("uploadImage")]
        [AuthorizeJwt]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _logger.LogInformation("Received request to upload image for account: {AccountId}", accountId);

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file uploaded for account: {AccountId}", accountId);
                return BadRequest("No file uploaded.");
            }

            var fileName = Path.GetFileName(file.FileName);

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    _logger.LogInformation("Uploading file: {FileName} for account: {AccountId}", fileName, accountId);

                    var fileUrl = await _accountImageService.UploadImageAsync(
                        new AccountImage() { AccountId = accountId, FileName = file.FileName },
                        stream
                    );

                    _logger.LogInformation("File uploaded successfully: {FileName} for account: {AccountId}", fileName, accountId);
                    return Ok(new { FileUrl = fileUrl });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName} for account: {AccountId}", fileName, accountId);
                return StatusCode(500, "An error occurred while uploading the file.");
            }
        }

        [HttpGet("api/images/")]
        [AuthorizeJwt]
        public async Task<IActionResult> GetImageAsync(string fileName)
        {
            int accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            _logger.LogInformation("Received request to get image: {FileName} for account: {AccountId}", fileName, accountId);

            try
            {
                byte[] fileBytes = await _accountImageService.GetImageByFileName(accountId, fileName);

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    _logger.LogWarning("File not found or empty: {FileName} for account: {AccountId}", fileName, accountId);
                    return NotFound("File not found.");
                }

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fileName, out var contentType))
                {
                    _logger.LogWarning("Could not determine content type for file: {FileName}. Defaulting to application/octet-stream.", fileName);
                    contentType = "application/octet-stream";
                }

                _logger.LogInformation("Successfully retrieved image: {FileName} for account: {AccountId}", fileName, accountId);
                return File(fileBytes, contentType);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "File not found exception: {FileName} for account: {AccountId}", fileName, accountId);
                return NotFound("File not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving image: {FileName} for account: {AccountId}", fileName, accountId);
                return StatusCode(500, "An error occurred while retrieving the image.");
            }
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
