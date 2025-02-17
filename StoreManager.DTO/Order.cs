using StoreManager.DTO.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace StoreManager.DTO;

public class Order : IDto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "EmployeeId is required.")]
    public int EmployeeId { get; set; }

    public int? CustomerId { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
    public string? Description { get; set; }

    public DateTime CreateDate { get; set; }

    public bool IsActive { get; set; }
}