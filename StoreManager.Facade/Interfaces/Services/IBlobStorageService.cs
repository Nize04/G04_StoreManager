namespace StoreManager.Facade.Interfaces.Services
{
    public interface IBlobStorageService:IAzureStorageService
    {
        string GenerateBlobUrl(string fileName);
        Task<byte[]> DownloadFile(string fileName);
    }
}