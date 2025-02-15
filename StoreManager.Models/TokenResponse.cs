namespace StoreManager.Models
{
    public class TokenResponse
    {
        public string Token { get; set; } = null!;
        public int AccountId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string RefreshToken { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
    }
}