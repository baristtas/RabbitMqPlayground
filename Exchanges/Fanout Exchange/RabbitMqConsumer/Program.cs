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

            var randomQueueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(randomQueueName, "logs-fanout", string.Empty, null);


            Console.WriteLine("listening " + randomQueueName);
            var consumer = new EventingBasicConsumer(channel);

            channel.BasicQos(0, 6, false); 

            channel.BasicConsume(randomQueueName, false, consumer);

            //Lambda implementation of receive slot
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var msg = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1000);
                Console.WriteLine("message received: " + msg);
                channel.BasicAck(e.DeliveryTag,true);
            };

            Console.ReadLine();
        }
    }
}