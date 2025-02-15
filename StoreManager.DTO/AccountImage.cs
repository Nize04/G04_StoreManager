

namespace StoreManager.DTO
{
    public class AccountImage
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string FileName { get; set; } = null!;
        public string BlobUrl { get; set; } = null!;
        public DateTime UploadedTime { get; set; }
        public bool IsActive { get; set; }

    }
}