using StoreManager.DTO.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace StoreManager.DTO;

public class SupplierTransaction : IDto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Supplier ID is required.")]
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "Employee ID is required.")]
    public int EmployeeId { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Transaction time is required.")]
    public DateTime TransactionTime { get; set; }

    public bool IsActive { get; set; }
}