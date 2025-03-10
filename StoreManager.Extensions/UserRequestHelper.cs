using DeviceDetectorNET;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StoreManager.Facade.Interfaces.Utilities;
using System.Net;

namespace StoreManager.Extensions
{
    public class UserRequestHelper:IUserRequestHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserRequestHelper> _logger;

        public UserRequestHelper(IHttpContextAccessor httpContextAccessor, ILogger<UserRequestHelper> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GetUserIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                _logger.LogWarning("⚠️ HttpContext is null. Cannot determine IP address.");
                return "Unknown";
            }

            string ipAddress = "Unknown";

            try
            {
                if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
                {
                    ipAddress = forwardedFor.ToString().Split(',').Select(ip => ip.Trim())
                        .FirstOrDefault(ip => !IsPrivateIp(ip)) ?? "Unknown";
                }

                if (ipAddress == "Unknown" && context.Connection.RemoteIpAddress != null)
                {
                    ipAddress = context.Connection.RemoteIpAddress.ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving user IP address.");
            }

            _logger.LogInformation("🌍 User IP Address: {IpAddress}", ipAddress);
            return ipAddress;
        }

        public string GetDeviceDetails()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                _logger.LogWarning("⚠️ HttpContext is null. Cannot determine device details.");
                return "Unknown";
            }

            var userAgent = context.Request.Headers["User-Agent"].ToString();
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                _logger.LogWarning("⚠️ User-Agent header is missing or empty.");
                return "Unknown";
            }

            var deviceDetector = new DeviceDetector(userAgent);
            deviceDetector.Parse();

            if (deviceDetector.IsBot())
            {
                var botName = deviceDetector.GetBot()?.ParserName ?? "Unknown Bot";
                _logger.LogWarning("🤖 Bot detected! Name: {BotName}, User-Agent: {UserAgent}", botName, userAgent);
                return $"Bot ({botName})";
            }

            var deviceType = deviceDetector.GetDeviceName() ?? "Unknown Device";
            var os = deviceDetector.GetOs()?.ParserName ?? "Unknown OS";
            var client = deviceDetector.GetClient()?.ParserName ?? "Unknown Client";

            var deviceInfo = $"Device: {deviceType}, OS: {os}, Client: {client}";
            _logger.LogInformation("📱 User Device Info: {DeviceInfo}", deviceInfo);

            return deviceInfo;
        }
        public string GetClientInfoFromDeviceInfo(string deviceInfo)
        {
            if (string.IsNullOrWhiteSpace(deviceInfo))
            {
                return "Unknown";
            }

            // Assuming the deviceInfo string is in the format: "Device: {deviceName}, OS: {osName}, Client: {clientName}"
            var clientPattern = "Client: ";
            var clientStartIndex = deviceInfo.IndexOf(clientPattern);

            if (clientStartIndex >= 0)
            {
                var clientInfo = deviceInfo.Substring(clientStartIndex + clientPattern.Length).Trim();

                if (!string.IsNullOrWhiteSpace(clientInfo))
                {
                    return clientInfo;
                }
            }

            return "Unknown";
        }

        private bool IsPrivateIp(string ip)
        {
            if (IPAddress.TryParse(ip, out var ipAddress))
            {
                byte[] bytes = ipAddress.GetAddressBytes();
                return (bytes[0] == 10) || 
                       (bytes[0] == 172 && (bytes[1] >= 16 && bytes[1] <= 31)) ||
                       (bytes[0] == 192 && bytes[1] == 168); 
            }
            return false;
        }
    }
}