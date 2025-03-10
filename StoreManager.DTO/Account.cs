using StoreManager.DTO.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace StoreManager.DTO;

public class Account : IDto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "username is required.")]
    [StringLength(22, ErrorMessage = "username cannot be longer than 22 characters.")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]

    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Salt is required.")]
    public string Salt { get; set; } = null!;

    public bool Requires2FA { get; set; }

    [Required]
    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    [Required]
    public bool IsActive { get; set; }
}
