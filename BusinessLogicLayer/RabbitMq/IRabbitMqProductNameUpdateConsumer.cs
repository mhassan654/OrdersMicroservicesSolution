namespace BusinessLogicLayer.RabbitMq
{
    public interface IRabbitMqProductNameUpdateConsumer
    {
        void Consume();
        void Dispose();
    }
}