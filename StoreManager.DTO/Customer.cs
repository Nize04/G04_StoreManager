using StoreManager.DTO.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace StoreManager.DTO;

public class Customer : IDto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(255, ErrorMessage = "Full name cannot be longer than 255 characters.")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string PhoneNumber { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool IsActive { get; set; }
}