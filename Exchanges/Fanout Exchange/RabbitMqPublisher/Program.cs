using RabbitMQ.Client;
using System.Text;

namespace RabbitMqPublisher
{
    internal class Program
    {
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
            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"FMessage {x}";

                var msgBody = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs-fanout","", null, msgBody);

                Console.WriteLine($"msg sent: {message}");
            });

            Console.ReadLine();
        }
    }
}