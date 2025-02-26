namespace StoreManager.Facade.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}