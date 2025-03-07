using StoreManager.DTO;

public interface IAccountQueryService
{
    Task<Account?> GetAccountByIdAsync(int id);
    Task<Account> GetAccountByEmailAsync(string email);
}
