using System.ComponentModel.DataAnnotations;

namespace StoreManager.Models
{
    public class CategoryModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}