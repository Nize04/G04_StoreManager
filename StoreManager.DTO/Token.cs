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
        public string TokenHash { get; set; } = null!;

        [Required(ErrorMessage = "Expiration date is required.")]
        public DateTime ExpiresAt { get; set; }

        [Required(ErrorMessage = "Create date is required.")]
        public DateTime CreateDate { get; set; }

        public DateTime? RevokedAt { get; set; }

        [StringLength(500, ErrorMessage = "Device info cannot exceed 500 characters.")]
        public string? DeviceInfo { get; set; }

        public bool IsActive { get; set; }
    }
}