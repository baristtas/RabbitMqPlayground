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
            channel.ExchangeDeclare("logs-topicExchange", durable: true, type: ExchangeType.Topic);


            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log1 = (LogNames)new Random().Next(1, 5);
                LogNames log2 = (LogNames)new Random().Next(1, 5);
                LogNames log3 = (LogNames)new Random().Next(1, 5);

                var routeKey = $"{log1}.{log2}.{log3}";

                string message = $"log-type: {log1}.{log2}.{log3} - {x}";
                var msgBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-topicExchange", routeKey,null,msgBody);

                Console.WriteLine($"msg sent: {message}");
            });

            Console.ReadLine();
        }
    }
}