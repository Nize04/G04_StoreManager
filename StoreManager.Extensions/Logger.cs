using Microsoft.Extensions.Logging;

namespace StoreManager.Extensions
{
    public class Logger<T> : ILogger<T>
    {
        private readonly string _categoryName;

        public Logger()
        {
            _categoryName = typeof(T).FullName ?? "Unknown";
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null!;
        }

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            string message = formatter(state, exception);
            Console.WriteLine($"{DateTime.Now} [{logLevel}] {_categoryName}: {message}");

            if (exception != null)
            {
                Console.WriteLine($"Exception: {exception.Message}");
            }
        }
    }
}