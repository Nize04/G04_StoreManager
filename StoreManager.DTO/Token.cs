using System.ComponentModel.DataAnnotations;

namespace StoreManager.DTO
{
    public class Token
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Account ID is required.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Token hash is required.")]
        [StringLength(512, ErrorMessage = "Token hash length cannot exceed 512 characters.")]
        public byte[] AccessTokenHash { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;

        [Required(ErrorMessage = "Access token expiration date is required.")]
        public DateTime AccessTokenExpiresAt { get; set; }

        [Required(ErrorMessage = "Refresh token expiration date is required.")]
        public DateTime RefreshTokenExpiresAt { get; set; }

        [Required(ErrorMessage = "Create date is required.")]
        public DateTime CreateDate { get; set; }

        public DateTime? RevokedAt { get; set; }

        [StringLength(500, ErrorMessage = "Device info cannot exceed 500 characters.")]
        public string DeviceInfo { get; set; } = null!;

        [Required(ErrorMessage = "IP address is required.")]
        [StringLength(45, ErrorMessage = "IP address cannot exceed 45 characters.")]
        public string IpAddress { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}