using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DnsClient.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BusinessLogicLayer.RabbitMq
{
    public class RabbitMqProductNameUpdateConsumer : IRabbitMqProductNameUpdateConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly IChannel _channel;
        private readonly IConnection _connection;
        private readonly ILogger<RabbitMqProductNameUpdateConsumer> _logger;

        public RabbitMqProductNameUpdateConsumer(
            IConfiguration configuration,
            IChannel channel,
            IConnection connection,
            ILogger<RabbitMqProductNameUpdateConsumer> logger
            )
        {
            _configuration = configuration;
            _channel = channel;
            _connection = connection;
            _logger = logger;

            string rabbitMqHostName = _configuration["RabbitMQ_HostName"]!;
            string rabbitMqUserName = _configuration["RabbitMQ_UserName"]!;
            string rabbitMPassword = _configuration["RabbitMQ_Password"]!;
            string rabbitMqPort = _configuration["RabbitMQ_Port"]!;

            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitMqHostName,
                UserName = rabbitMqUserName,
                Password = rabbitMPassword,
                Port = Convert.ToInt32(rabbitMqPort)
            };

            // Fix: Use RabbitMq.Client's IConnection and ensure proper namespace is referenced
            _connection = connectionFactory.CreateConnectionAsync().Result;

            // Fix: Use the CreateModel method from RabbitMq.Client's IConnection
            _channel = _connection.CreateChannelAsync().Result;
        }

        public void Consume()
        {
            string routingKey = "product.update.name";
            string queueName = "orders.product.update.name.queue";

            // create exchange
            string exchangeName = _configuration["RabbitMQ_Products_Exchange"]!;

            _channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false,
                arguments: null
            );

            // create message queue
            _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // bind the message to exchange
            _channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey
            );

            // Initialize the consumer properly
            var consumer = new AsyncEventingBasicConsumer(_channel);

            // Attach the event handler
            consumer.ReceivedAsync += async (sender, args) =>
            {
                try
                {
                    byte[] body = args.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);

                    if (!string.IsNullOrEmpty(message))
                    {
                        ProductNameUpdateMessage productNameUpdateMessage =
                            JsonSerializer.Deserialize<ProductNameUpdateMessage>(message);

                        _logger.LogInformation(
                            $"Product name updated: {productNameUpdateMessage.ProductID}, " +
                            $"New name: {productNameUpdateMessage.NewName}"
                        );
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                }

                // Ensure the lambda returns a Task as required by AsyncEventHandler
                await Task.CompletedTask;
            };

            // Start consuming messages
            _channel.BasicConsumeAsync(queue: queueName, consumer: consumer, autoAck: true);
        }

       

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
