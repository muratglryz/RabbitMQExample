
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQExample.Models;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitMQExample.Services
{
    public class ProductImageBackgroudService : BackgroundService
    {
        private readonly RabbitMQClient _rabbitmqClient;
        private readonly ILogger<ProductImageBackgroudService> _logger;
        private IModel _channel;
        public ProductImageBackgroudService( RabbitMQClient rabbitmqClient, ILogger<ProductImageBackgroudService> logger)
        {
            _rabbitmqClient = rabbitmqClient;
            _logger = logger;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitmqClient.Connect();

            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }



        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            _channel.BasicConsume(RabbitMQClient.QueueName, false, consumer);

            consumer.Received += Consumer_Received;
            return Task.CompletedTask;
        }
        private async Task<Task> Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {

            try
            {
               
                var productImageCreatedEvent = JsonSerializer.Deserialize<NoneImage>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", "sample_picture.jpg");
                using var img = Image.FromFile(path);
                img.Save("wwwroot/Images/" + productImageCreatedEvent.ImageName);
                img.Dispose();
                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return  Task.CompletedTask;

        }
    }
}
