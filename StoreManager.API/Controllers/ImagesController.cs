using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MyAttributes;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Services;
using System.Security.Claims;

namespace StoreManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeJwt]
    public class ImagesController : ControllerBase
    {
        private readonly IAccountImageService _accountImageService;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IAccountImageService accountImageService,
            ILogger<ImagesController> logger)
        {
            _accountImageService = accountImageService ?? throw new ArgumentNullException(nameof(accountImageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("uploadImage")]
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
    }
}