using Microsoft.EntityFrameworkCore.Metadata;
using NuGet.Protocol.Plugins;
using RabbitMQ.Client;

using IConnection = RabbitMQ.Client.IConnection;
using IModel = RabbitMQ.Client.IModel;

namespace ImageWatermarkRabbitMQ.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly RabbitMQ.Client.ConnectionFactory m_connectionFactory;
        private IConnection m_connection;
        private IModel m_channel;

        public static string exchangeName = "ImageDirectExchange";
        public static string routingWatermark = "watermark-route-image";
        public static string queueName = "queue-watermark-image";

        private readonly ILogger<RabbitMQClientService> m_logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            m_connectionFactory = connectionFactory;
            m_logger= logger;
            connect();
        }

        public IModel connect()
        {
            m_connection= m_connectionFactory.CreateConnection();

            if(m_channel is { IsOpen: true })
            {
                return m_channel;
            }

            m_channel = m_connection.CreateModel();

            m_channel.ExchangeDeclare(exchangeName, type: "direct", true, false);
            m_channel.QueueDeclare(queueName, true, false, false);

            m_channel.QueueBind(queueName, exchangeName, routingWatermark);

            m_logger.LogInformation("Connection created.");

            return m_channel;

        }

        public void Dispose()
        {
            m_channel?.Close();
            m_channel?.Dispose();
            m_channel = default;

            m_connection?.Close();
            m_connection?.Dispose();
            m_connection = default;

            m_logger.LogInformation("Channel and connection disposed!");
        }
    }
}
