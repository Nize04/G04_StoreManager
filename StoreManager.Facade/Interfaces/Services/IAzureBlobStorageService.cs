namespace StoreManager.Facade.Interfaces.Services
{
     public interface IAzureBlobStorageService:IAzureStorageService
     {
         string GenerateBlobUrl(string fileName);
         Task<byte[]> DownloadFile(string fileName);
     }
}
