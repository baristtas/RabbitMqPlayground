using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelFileCreateWorkerService.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly RabbitMQ.Client.ConnectionFactory m_connectionFactory;
        private IConnection m_connection;
        private IModel m_channel;

        public static string exchangeName = "ExcelDirectExchange";
        public static string routingExcel = "excel-route-file";
        public static string queueName = "queue-excel-file";

        private readonly ILogger<RabbitMQClientService> m_logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            m_connectionFactory = connectionFactory;
            m_logger = logger;
        }

        public IModel connect()
        {
            m_connection = m_connectionFactory.CreateConnection();

            if (m_channel is { IsOpen: true })
            {
                return m_channel;
            }

            m_channel = m_connection.CreateModel();

            m_channel.ExchangeDeclare(exchangeName, type: "direct", true, false);
            m_channel.QueueDeclare(queueName, true, false, false);

            m_channel.QueueBind(queueName, exchangeName, routingExcel);

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
