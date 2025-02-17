
using System.ComponentModel.DataAnnotations;

namespace StoreManager.DTO
{
    public class ProductsQuantity
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}