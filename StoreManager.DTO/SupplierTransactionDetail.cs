using System.ComponentModel.DataAnnotations;

namespace StoreManager.DTO;

public class SupplierTransactionDetail
{
    [Key]
    public int SupplierTransactionId { get; set; }

    [Required(ErrorMessage = "Product ID is required.")]
    public int ProductId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Production date is required.")]
    public DateTime ProductionDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool IsActive { get; set; }
}