using ContentService.Helpers;
using ContentService.Interfaces;

namespace ContentService.Managers
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly LogHelper<RabbitMQListener> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        //private readonly RabbitMQConnection _rabbitMqConnection;

        public RabbitMQListener(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = new LogHelper<RabbitMQListener>();
            //_rabbitMqConnection = rabbitMQConnection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInfo("RabbitMQ Listener Service is starting...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Create a scope for scoped services
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var examPracticeManager = scope.ServiceProvider.GetRequiredService<IExamPracticeManager>();

                        // Call the listening method inside the scope
                        await examPracticeManager.StartListeningAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in RabbitMQ Listener Service", ex);
                }

                await Task.Delay(5000, stoppingToken); // Prevents infinite crash loops
            }

            _logger.LogInfo("RabbitMQ Listener Service is stopping...");
        }
    }
}
