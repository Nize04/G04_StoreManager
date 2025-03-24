using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;

namespace StoreManager.Services
{
    public class AzureBlobPhotoStorageService : AzureBlobStorageService,IAzureBlobPhotoStorageService
    {
        public AzureBlobPhotoStorageService(IOptions<AzureBlobPhotoStorageSettings> options, ILogger<AzureBlobPhotoStorageService> logger) : base(options, logger)
        {
        }
    }
}