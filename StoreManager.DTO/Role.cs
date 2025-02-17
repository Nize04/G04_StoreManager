
using System.ComponentModel.DataAnnotations;

namespace StoreManager.DTO
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(100, ErrorMessage = "Role name cannot be longer than 100 characters.")]
        public string RoleName { get; set; } = null!;

        public DateTime CreateDate { get; set; }

        public bool IsActive { get; set; }
    }
}