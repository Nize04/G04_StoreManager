using Microsoft.Extensions.Options;
using StoreManager.Facade.Interfaces.Services;
using StoreManager.Models;

namespace StoreManager.Services;
public abstract class AzureStorageServiceBase:IAzureStorageService
{
    protected readonly AzureStorageSettings _storageSettings;

    protected AzureStorageServiceBase(IOptions<AzureStorageSettings> options)
    {
        if (options?.Value == null)
        {
            throw new ArgumentNullException(nameof(options), "Azure Storage settings cannot be null.");
        }

        _storageSettings = options.Value;
    }

    public abstract Task<string> UploadFileAsync(Stream fileStream, string fileName);
    public abstract Task<bool> DeleteFileAsync(string fileName);
    public abstract string GetFileUrl(string fileName);
}