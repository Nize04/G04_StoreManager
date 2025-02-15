using StoreManager.DTO.Interfaces;
namespace StoreManager.DTO;

public class Category:IDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? UpdateDate { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsActive { get; set; }
}