using StoreManager.DTO;

namespace StoreManager.Facade.Interfaces.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<bool> AuthenticateAsync(string email, string password);
    }
}