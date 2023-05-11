using RabbitMQ.Client;
using System.Text;

namespace RabbitMqPublisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://zgvcyqrk:FAAZd2v-1z-jl4vKNUoizuh_XFrxKHcR@hawk.rmq.cloudamqp.com/zgvcyqrk");

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