using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IAccountVideoService
    {
        Task<int> UploadVideoAsync(AccountVideo accountVideo, Stream fileStream);
        Task<byte[]> GetVideoByFileName(int accountId, string fileName);
    }
}