using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;

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

            // Ensure connection is established when the class is instantiated
            try
            {
                _connection = _factory.CreateConnectionAsync().GetAwaiter().GetResult();
                Log.Information("Connected to RabbitMQ");
                _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
                Log.Information("Channel created in RabbitMQ");
            }
            catch (BrokerUnreachableException ex)
            {
                Log.Error(ex, "RabbitMQ connection error");
                throw;
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
