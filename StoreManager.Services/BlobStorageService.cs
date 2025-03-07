using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;

namespace StoreManager.Services;

public class BlobStorageService : AzureStorageServiceBase, IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<BlobStorageService> _logger;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;
    private const string DefaultContentType = "application/octet-stream";
    private const long MaxFileSizeInBytes = 15 * 1024 * 1024;

    public BlobStorageService(IOptions<AzureStorageSettings> options, ILogger<BlobStorageService> logger)
        : base(options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _contentTypeProvider = new FileExtensionContentTypeProvider();

        var blobServiceClient = new BlobServiceClient(_storageSettings.ConnectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(_storageSettings.ContainerName);

        EnsureContainerExists();
    }

    public override async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            if (fileStream.Length > MaxFileSizeInBytes)
            {
                throw new InvalidOperationException("File size exceeds the maximum limit.");
            }

            if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
            {
                contentType = DefaultContentType;
            }

            var blobClient = _containerClient.GetBlobClient(fileName);

            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            };

            await blobClient.UploadAsync(fileStream, uploadOptions);
            _logger.LogInformation("File uploaded: {FileName} with Content-Type: {ContentType}", fileName, contentType);

            return blobClient.Uri.AbsoluteUri;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<byte[]> DownloadFile(string fileName)
    {
        try
        {
            _logger.LogInformation("Downloading file: {FileName}", fileName);

            var blobClient = _containerClient.GetBlobClient(fileName); 
            Azure.Response<BlobDownloadResult> response = await blobClient.DownloadContentAsync();

            _logger.LogInformation("File {FileName} downloaded successfully.", fileName);

            return response.Value.Content.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FileName}", fileName);
            throw;
        }
    }

    public override async Task<bool> DeleteFileAsync(string fileName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var response = await blobClient.DeleteIfExistsAsync();

            if (response.Value)
            {
                _logger.LogInformation("File deleted successfully: {FileName}", fileName);
            }
            else
            {
                _logger.LogWarning("File deletion failed (not found): {FileName}", fileName);
            }

            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FileName}", fileName);
            throw;
        }
    }

    public override string GetFileUrl(string fileName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var fileUrl = blobClient.Uri.ToString();

            _logger.LogInformation("File URL retrieved: {FileName} -> {FileUrl}", fileName, fileUrl);
            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file URL: {FileName}", fileName);
            throw;
        }
    }
    public string GenerateBlobUrl(string fileName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating Blob URL for file: {FileName}", fileName);
            throw;
        }
    }

    private void EnsureContainerExists()
    {
        try
        {
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
            _logger.LogInformation("Blob container initialized: {ContainerName}", _storageSettings.ContainerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Blob container: {ContainerName}", _storageSettings.ContainerName);
        }
    }
}