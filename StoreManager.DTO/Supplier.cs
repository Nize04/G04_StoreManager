using StoreManager.DTO.Interfaces;
namespace StoreManager.DTO;

public class Supplier : IDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public bool IsActive { get; set; }
}