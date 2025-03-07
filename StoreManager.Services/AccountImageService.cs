using Microsoft.Extensions.Logging;
using StoreManager.DTO;
using StoreManager.Facade.Interfaces.Repositories;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class AccountImageService : IAccountImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountImageService> _logger;
        private readonly IBlobStorageService _blobStorageService;

        public AccountImageService(IUnitOfWork unitOfWork,
            ILogger<AccountImageService> logger,
            IBlobStorageService blobStorageService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
        }

        public async Task<int> UploadImageAsync(AccountImage accountImage, Stream fileStream)
        {
            if (accountImage == null) throw new ArgumentNullException(nameof(accountImage));
            if (fileStream == null || fileStream.Length == 0) throw new ArgumentException("Invalid file stream.", nameof(fileStream));

            try
            {
                _logger.LogInformation("Starting upload for image with AccountId: {AccountId}", accountImage.AccountId);

                accountImage.BlobUrl = _blobStorageService.GenerateBlobUrl(accountImage.FileName);

                await _unitOfWork.OpenConnectionAsync();

                await _unitOfWork.BeginTransactionAsync();

                int result = (int)await _unitOfWork.AccountImageRepository.InsertAsync(accountImage);

                await _blobStorageService.UploadFileAsync(fileStream, accountImage.FileName);

                await _unitOfWork.CommitAsync();
                _logger.LogInformation("Image uploaded and metadata saved for AccountId: {AccountId}", accountImage.AccountId);

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollBackAsync();

                _logger.LogError(ex, "An error occurred while uploading image for AccountId: {AccountId}", accountImage.AccountId);
                throw;
            }
            finally
            {
                await _unitOfWork.CloseConnectionAsync();
            }
        }

        public async Task<byte[]> GetImageByFileName(int accountId, string fileName)
        {
            try
            {
                _logger.LogInformation("Fetching image: {FileName} for account: {AccountId}", fileName, accountId);

                var accountImage = await _unitOfWork.AccountImageRepository
                    .GetAsync(a => a.AccountId == accountId && a.FileName == fileName && a.IsActive == true);

                var imageRecord = accountImage.FirstOrDefault();
                if (imageRecord == null)
                {
                    _logger.LogWarning("No active image found for file: {FileName} and account: {AccountId}", fileName, accountId);
                    throw new FileNotFoundException($"Image '{fileName}' not found or not accessible.");
                }

                var fileBytes = await _blobStorageService.DownloadFile(fileName);

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    _logger.LogError("Blob storage returned an empty file for: {FileName}", fileName);
                    throw new InvalidOperationException($"File '{fileName}' is empty or could not be retrieved.");
                }

                _logger.LogInformation("Successfully retrieved image: {FileName} for account: {AccountId}", fileName, accountId);
                return fileBytes;
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "File not found: {FileName} for account: {AccountId}", fileName, accountId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving image: {FileName} for account: {AccountId}", fileName, accountId);
                throw;
            }
        }
    }
}