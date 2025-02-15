using System.ComponentModel.DataAnnotations;

namespace StoreManager.Models
{
    public class EmployeeModel
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        public int? ReportsTo { get; set; }
    }
}