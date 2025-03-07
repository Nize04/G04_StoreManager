namespace StoreManager.Facade.Interfaces.Services
{
    public interface IAzureStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName);
        Task<bool> DeleteFileAsync(string fileName);
        string GetFileUrl(string fileName);
    }
}