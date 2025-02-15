using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Facade.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
