namespace StoreManager.Models
{
    public class AccountModel
    {
        public string Email { get; set; } = null!;
        public IEnumerable<RoleModel> Roles { get; set; } = null!;
    }
}