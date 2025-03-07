using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IAccountImageService
    {
        Task<int> UploadImageAsync(AccountImage accountImage, Stream fileStream);
        Task<byte[]> GetImageByFileName(int accountId, string fileName);
    }
}