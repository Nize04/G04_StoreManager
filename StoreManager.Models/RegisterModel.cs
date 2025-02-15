
using System.ComponentModel.DataAnnotations;

namespace StoreManager.Models
{
    public class RegisterModel
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}