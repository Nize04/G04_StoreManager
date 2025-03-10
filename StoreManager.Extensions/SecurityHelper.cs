namespace StoreManager.Extensions
{
    public static class SecurityHelper
    {
        private static readonly List<string> KnownBots = new()
        {
            "Googlebot", "Bingbot", "Yahoo! Slurp", "DuckDuckBot", "Baiduspider", "YandexBot",
            "facebot", "ia_archiver", "bot", "crawler", "spider", "python-requests", "PostmanRuntime"
        };

        private static readonly List<string> SuspiciousIps = new()
        {
            "123.45.67.89", "98.76.54.32"
        };

        public static bool IsKnownBot(string userAgent)
        {
            return !string.IsNullOrEmpty(userAgent) && KnownBots.Any(bot => userAgent.Contains(bot, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsSuspiciousIp(string ipAddress)
        {
            return SuspiciousIps.Contains(ipAddress);
        }
    }
}