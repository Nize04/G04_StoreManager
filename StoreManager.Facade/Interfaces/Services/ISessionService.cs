using StoreManager.DTO;
using StoreManager.Models;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface ISessionService
    {
        void CustomeSession(IDictionary<string, object> items);
        string GetString(string itemName);
        int? GetInt32(string itemName);
        byte[] Get(string itemName);
        void Remove(string itemName);
        void Clear();
    }
}