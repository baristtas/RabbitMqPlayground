using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;

namespace RabbitMqConsumer
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

            var queueName = channel.QueueDeclare().QueueName;

            channel.ExchangeDeclare("logs-topicExchange", durable: true, type: ExchangeType.Topic);

            var routeKey = "Warning.*.Critical";
            channel.QueueBind(queueName, "logs-topicExchange", routeKey, null);

            Console.WriteLine("listening " + queueName);
            var consumer = new EventingBasicConsumer(channel);


            channel.QueueBind(queueName, "logs-direct", queueName, null);

            channel.BasicConsume(queueName, false, consumer);

            //Lambda implementation of receive slot
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var msg = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1000);
                Console.WriteLine("message received: " + msg);
                File.AppendAllText("logs-critical.txt", msg + "\n");
                channel.BasicAck(e.DeliveryTag,true);
            };

            Console.ReadLine();
        }
    }
}