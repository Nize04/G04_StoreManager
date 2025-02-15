using DeviceDetectorNET;
using Microsoft.AspNetCore.Http;

namespace StoreManager.Extensions
{
    public static class UserRequestHelper
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void Configure(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }

        public static string? GetUserIpAddress()
        {
            var context = _httpContextAccessor?.HttpContext;

            if (context?.Request.Headers.ContainsKey("X-Forwarded-For") == true)
            {
                return context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
            }

            return context?.Connection.RemoteIpAddress?.ToString();
        }

        public static string GetDeviceDetails()
        {
            var context = _httpContextAccessor?.HttpContext;

            if (context == null)
                return "Unknown";

            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var deviceDetector = new DeviceDetector(userAgent);
            deviceDetector.Parse();

            if (deviceDetector.IsBot())
            {
                return "Bot";
            }

            // You may want to add more verbose or specialized information about device type and OS
            var deviceType = deviceDetector.GetDeviceName() ?? "Unknown Device";
            var os = deviceDetector.GetOs()?.ParserName ?? "Unknown OS";
            var client = deviceDetector.GetClient()?.ParserName ?? "Unknown Client";

            return $"Device: {deviceType}, OS: {os}, Client: {client}";
        }
    }
}
