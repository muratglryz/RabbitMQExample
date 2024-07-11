using RabbitMQ.Client;
using RabbitMQExample.Models;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitMQExample.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClient _client;
        public RabbitMQPublisher(RabbitMQClient rabbitMQClient)
        {
            _client = rabbitMQClient;
        }
        public void Publish(NoneImage noneImagesId)
        {
            var channel = _client.Connect();

            var bodyString = JsonSerializer.Serialize(noneImagesId);

            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: RabbitMQClient.ExchangeName, routingKey: RabbitMQClient.RoutingName, basicProperties: properties, body: bodyByte);

        }
    }
}
