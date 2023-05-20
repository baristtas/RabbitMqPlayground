using RabbitMQ.Client;
using System.Text;

namespace RabbitMqPublisher
{
    internal class Program
    {
        public enum LogNames
        {
            Critical = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        };

        static void Main(string[] args)
        {
            string uri = "";
            try
            {
                StreamReader sr = new StreamReader("C:\\Users\\baris.tas\\Desktop\\rbmq\\amqpinstanceuri.txt");
                uri = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            var factory = new ConnectionFactory();
            factory.Uri = new Uri(uri);

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var routeKey = $"route-{x}";

                var queueName = $"direct-exchange{x}";
                channel.QueueDeclare(queueName, true, false, false, null);
                channel.QueueBind(queueName,"logs-direct", routeKey);
            }
            );

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames logName = (LogNames) new Random().Next(1, 5);

                string message = $"log-type: {logName}-{x}";

                var msgBody = Encoding.UTF8.GetBytes(message);

                var routeKey = $"route-{logName}";

                channel.BasicPublish("logs-direct",routeKey,null,msgBody);

                Console.WriteLine($"msg sent: {message}");
            });

            Console.ReadLine();
        }
    }
}