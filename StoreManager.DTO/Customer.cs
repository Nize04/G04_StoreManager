using StoreManager.DTO.Interfaces;
namespace StoreManager.DTO;

public class Customer : IDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsActive { get; set; }
}