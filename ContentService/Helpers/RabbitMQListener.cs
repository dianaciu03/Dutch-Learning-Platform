using System.Text;
using ContentService.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace ContentService.Helpers
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly RabbitMQConnection _rabbitMqConnection;
        private readonly IServiceScopeFactory _scopeFactory;
        private IChannel _channel;
        private const string QueueName = "accountQueue";

        public RabbitMQListener(IServiceScopeFactory scopeFactory, RabbitMQConnection rabbitMQConnection)
        {
            _scopeFactory = scopeFactory;
            _rabbitMqConnection = rabbitMQConnection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Starting RabbitMQ listener...");

            _channel = _rabbitMqConnection.GetChannel();

            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += HandleMessageAsync;

            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: true,
                consumer: consumer);

            Log.Information("RabbitMQ listener is now consuming messages.");
        }

        private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Log.Information("Received message: {Message}", message);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var examPracticeManager = scope.ServiceProvider.GetRequiredService<IExamPracticeManager>();

                var accountId = ExtractAccountId(message);
                if (accountId.HasValue)
                {
                    Log.Information("Processing Account ID: {AccountId}", accountId);
                    // Optional: await examPracticeManager.HandleAccountDeleted(accountId.Value);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error handling message");
            }
        }

        private Guid? ExtractAccountId(string message)
        {
            if (message.StartsWith("Account to delete:"))
            {
                var parts = message.Split(':');
                if (parts.Length > 1 && Guid.TryParse(parts[1].Trim(), out var id))
                {
                    return id;
                }
            }
            return null;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("Stopping RabbitMQ listener...");

            if (_channel != null)
            {
                await _rabbitMqConnection.CloseAsync();
            }

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _rabbitMqConnection?.Dispose();
            base.Dispose();
        }
    }
}
