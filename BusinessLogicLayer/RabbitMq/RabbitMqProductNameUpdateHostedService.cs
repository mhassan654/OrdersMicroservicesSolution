

using Microsoft.Extensions.Hosting;

namespace BusinessLogicLayer.RabbitMq
{
    public class RabbitMqProductNameUpdateHostedService : IHostedService
    {
        private readonly IRabbitMqProductNameUpdateConsumer _consumer;

        public RabbitMqProductNameUpdateHostedService(IRabbitMqProductNameUpdateConsumer consumer)
        {
            _consumer = consumer;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _consumer.Consume();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Dispose();
            return Task.CompletedTask;  
        }
    }
}
