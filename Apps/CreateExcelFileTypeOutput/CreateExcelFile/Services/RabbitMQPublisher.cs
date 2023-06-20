using ImageWatermarkRabbitMQ.Services;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CreateExcelFile.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService m_rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            m_rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish(CreateExcelMessage excelCreatedEvent)
        {
            var channel = m_rabbitMQClientService.connect();

            var bodyStr = JsonSerializer.Serialize(excelCreatedEvent);

            var bodyByte = Encoding.UTF8.GetBytes(bodyStr);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(RabbitMQClientService.exchangeName, RabbitMQClientService.routingExcel, properties, bodyByte);
        }
    }
}
