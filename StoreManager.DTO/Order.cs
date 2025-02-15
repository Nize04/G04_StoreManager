using StoreManager.DTO.Interfaces;
namespace StoreManager.DTO;

public class Order : IDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int? CustomerId { get; set; }
    public string? Description { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsActive { get; set; }
}