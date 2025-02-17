using System.ComponentModel.DataAnnotations;

namespace StoreManager.DTO;

public class OrderDetail
{
    [Key]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "ProductId is required.")]
    public int ProductId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "UnitPrice must be greater than 0.")]
    public decimal UnitPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    public bool IsActive { get; set; }
}