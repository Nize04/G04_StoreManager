using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;

namespace StoreManager.Services
{
    public class AzureBlobVideoStorageService : AzureBlobStorageService,IAzureBlobVideoStorageService
    {
        public AzureBlobVideoStorageService(IOptions<AzureBlobVideoStorageSettings> options, ILogger<AzureBlobVideoStorageService> logger) : base(options, logger)
        {
            
        }
    }
}