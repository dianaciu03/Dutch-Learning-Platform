using Microsoft.Extensions.Logging;

namespace ContentService.Helpers
{
    public class LogHelper<T>
    {
        private ILogger<T> _logger;

        public LogHelper()
        {
            InitializeLogger();
        }

        // Method to initialize the logger (can be called manually to reconfigure if needed)
        private void InitializeLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole(); // You can add other providers or modify this configuration as needed
            });

            _logger = loggerFactory.CreateLogger<T>();
        }

        public void LogInfo(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        public void LogError(string message, Exception ex = null, params object[] args)
        {
            if (ex != null)
                _logger.LogError(ex, message, args);
            else
                _logger.LogError(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }

        public void LogCritical(string message, Exception ex = null, params object[] args)
        {
            if (ex != null)
                _logger.LogCritical(ex, message, args);
            else
                _logger.LogCritical(message, args);
        }
    }
}
