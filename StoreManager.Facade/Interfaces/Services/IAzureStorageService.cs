using Microsoft.AspNetCore.Http;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IAzureStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
        string GetBlobUrl(string fileName);
    }
}