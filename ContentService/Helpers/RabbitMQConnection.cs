using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace ContentService.Helpers
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly LogHelper<RabbitMQConnection> _logger;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQConnection(string hostName, string userName, string password)
        {
            _logger = new LogHelper<RabbitMQConnection>();
            _factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            // Ensure connection is established when the class is instantiated
            Task.Run(async () => await ConnectAsync()).GetAwaiter().GetResult();
        }

        public async Task ConnectAsync()
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync(); // Newer versions use this
                _logger.LogInfo("Connected to RabbitMQ.");
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError("RabbitMQ connection error: {0}", ex);
            }
        }

        public void PublishMessage(string message, string queueName)
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("Channel is not established.");
            }

            // Ensure the queue exists (optional)
            _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(message);

            var properties = new BasicProperties(); // Use BasicProperties directly
            properties.Persistent = true;  // Make sure the message is persisted

            // Publish the message to the queue
            Task.Run(async () =>
            {
                await _channel.BasicPublishAsync(exchange: "",
                                               routingKey: queueName,
                                               mandatory: false,
                                               basicProperties: properties,
                                               body: body);

                _logger.LogInfo("Sent message: {0}", message);
            });
        }

        public IChannel GetChannel()
        {
            if (_channel == null)
            {
                throw new InvalidOperationException("Connection is not established.");
            }
            return _channel;
        }

        public async Task CloseAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
            }
            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
        }

        public void Dispose()
        {
            CloseAsync().GetAwaiter().GetResult();
        }
    }
}
