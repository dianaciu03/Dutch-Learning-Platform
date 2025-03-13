using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace ContentService.Helpers
{
    public class RabbitMQConnection : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQConnection(string hostName, string userName, string password)
        {
            _factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };
        }

        public async Task ConnectAsync()
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync(); // Newer versions use this
                Console.WriteLine("Connected to RabbitMQ.");
            }
            catch (BrokerUnreachableException ex)
            {
                Console.WriteLine($"RabbitMQ connection error: {ex.Message}");
            }
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
