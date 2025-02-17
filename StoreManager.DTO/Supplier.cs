using StoreManager.DTO.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace StoreManager.DTO;

public class Supplier : IDto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(200, ErrorMessage = "Company name cannot be longer than 200 characters.")]
    public string CompanyName { get; set; } = null!;

    [Required(ErrorMessage = "Country is required.")]
    [StringLength(100, ErrorMessage = "Country name cannot be longer than 100 characters.")]
    public string Country { get; set; } = null!;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(100, ErrorMessage = "City name cannot be longer than 100 characters.")]
    public string City { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }

    public bool IsActive { get; set; }
}