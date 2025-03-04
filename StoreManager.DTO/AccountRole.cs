using System.ComponentModel.DataAnnotations;

namespace StoreManager.DTO
{
    public class AccountRole
    {
        [Key]
        public int AccountId { get; set; }
        [Key]
        public int RoleId { get; set; }
    }
}