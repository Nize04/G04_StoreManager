namespace StoreManager.DTO
{
    public class Token
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string TokenHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? DeviceInfo { get; set; }
        public bool IsActive { get; set; }
    }
}