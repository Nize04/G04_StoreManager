namespace StoreManager.Facade.Interfaces.Utilities
{
    public interface IUserRequestHelper
    {
        string GetUserIpAddress();
        string GetDeviceDetails();
        string GetClientInfoFromDeviceInfo(string deviceInfo);
    }
}