using StoreManager.DTO;
using StoreManager.Models;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ISessionService
    {
        void CustomeSession(IDictionary<string, object> items);
    }
}