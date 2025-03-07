using System.ComponentModel.DataAnnotations;

namespace StoreManager.Models
{
    public class AccountImageModel
    {
        [Required(ErrorMessage = "FileName is required.")]
        [StringLength(255, ErrorMessage = "FileName cannot be longer than 255 characters.")]
        public string FileName { get; set; } = null!;
    }
}