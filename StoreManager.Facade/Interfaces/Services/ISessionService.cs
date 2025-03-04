namespace StoreManager.Facade.Interfaces.Services
{
    public interface ISessionService
    {
        void CustomSession(IDictionary<string, object> items);
        string GetString(string itemName);
        int? GetInt32(string itemName);
        byte[] Get(string itemName);
        void Remove(string itemName);
        void Clear();
    }
}