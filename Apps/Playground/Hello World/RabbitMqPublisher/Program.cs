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

            channel.QueueDeclare("hello-queue", true, false, false, null);

            string message = "hello world";

            var msgBody = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(string.Empty, "hello-queue", null, msgBody);

            Console.WriteLine("msg sent");

            Console.ReadLine();
        }
    }
}