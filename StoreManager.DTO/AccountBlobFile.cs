using System.ComponentModel.DataAnnotations;

namespace StoreManager.DTO
{
    public class AccountBlobFile
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "AccountId is required.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "FileName is required.")]
        [StringLength(255, ErrorMessage = "FileName cannot be longer than 255 characters.")]
        public string FileName { get; set; } = null!;

        [Required(ErrorMessage = "BlobUrl is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string BlobUrl { get; set; } = null!;

        public DateTime UploadedDate { get; set; }

        public bool IsActive { get; set; }
    }
}