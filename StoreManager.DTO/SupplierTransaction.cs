using StoreManager.DTO.Interfaces;
namespace StoreManager.DTO;

public class SupplierTransaction : IDto
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime TransactionTime { get; set; }
    public bool IsActive { get; set; }
}