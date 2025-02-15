using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using StoreManager.Facade.Interfaces.Services;

namespace StoreManager.Services
{
    public class AzureStorageService : IAzureStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureStorageService(string connectionString, string containerName)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _containerName = containerName ?? throw new ArgumentNullException(nameof(containerName));
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await containerClient.CreateIfNotExistsAsync();

            var blobName = Guid.NewGuid().ToString();
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.AbsoluteUri;
        }

        public string GetBlobUrl(string fileName)
        {
            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            return blobClient.Uri.ToString();
        }
    }
}