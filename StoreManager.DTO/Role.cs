
namespace StoreManager.DTO
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }
    }
}