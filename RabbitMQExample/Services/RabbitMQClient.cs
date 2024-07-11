using RabbitMQ.Client;

namespace RabbitMQExample.Services
{
    public class RabbitMQClient
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageSaveDirectExchange";
        public static string RoutingName = "route-image-save";
        public static string QueueName = "queue-image-save";
        private readonly ILogger<RabbitMQClient> _logger;
        public RabbitMQClient(ConnectionFactory connectionFactory, ILogger<RabbitMQClient> logger)

        {
            _connectionFactory = connectionFactory;
            _logger = logger;
      
        }
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, type: "direct", true, false);
            _channel.QueueDeclare(QueueName, true, false, false, null);
            _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingName);
            _logger.LogInformation("RabbitMQ Bağlandı.");
            return _channel;
        }
        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _channel = default;
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("RabbitMQ Bağlantısı Koptu.");
        }
    }
}
