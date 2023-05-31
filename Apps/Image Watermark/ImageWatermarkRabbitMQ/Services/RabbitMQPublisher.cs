using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ImageWatermarkRabbitMQ.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService m_rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            m_rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent) 
        {
            var channel = m_rabbitMQClientService.connect();

            var bodyStr = JsonSerializer.Serialize(productImageCreatedEvent);

            var bodyByte = Encoding.UTF8.GetBytes(bodyStr);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(RabbitMQClientService.exchangeName,RabbitMQClientService.routingWatermark,properties,bodyByte);
        }
    }
}
