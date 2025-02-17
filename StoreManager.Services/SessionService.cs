using Microsoft.AspNetCore.Http;
using StoreManager.Facade.Interfaces.Services;


namespace StoreManager.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void Clear() => _httpContextAccessor.HttpContext.Session.Clear();

        public void CustomeSession(IDictionary<string, object> items)
        {
            var context = _httpContextAccessor.HttpContext;
            for (int i = 0; i < items.Count; i++)
            {
                var key = items.Keys.ElementAt(i);
                var value = items[key];
                if (value is string)
                {
                    context.Session.SetString(key, (string)value);
                }
                else context.Session.SetInt32(key, (int)value);
            }
        }
    }
}