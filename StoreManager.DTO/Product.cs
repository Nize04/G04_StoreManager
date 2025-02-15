using StoreManager.DTO.Interfaces;
namespace StoreManager.DTO;

public class Product : IDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsActive { get; set; }
}