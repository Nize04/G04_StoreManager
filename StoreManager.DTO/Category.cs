using StoreManager.DTO.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace StoreManager.DTO;

public class Category : IDto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters.")]
    public string Name { get; set; } = null!;

    public DateTime? UpdateDate { get; set; }

    public DateTime CreateDate { get; set; }

    public bool IsActive { get; set; }
}