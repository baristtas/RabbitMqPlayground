using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMqConsumer
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

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume("hello-queue", true, consumer);

            consumer.Received += MessageReceived;

            //Lambda implementation of receive slot
            //consumer.Received += (object sender, BasicDeliverEventArgs e) =>
            //{
            //    
            //};

            Console.ReadLine();
        }

        private static void MessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            var msg = Encoding.UTF8.GetString(e.Body.ToArray());

            Console.WriteLine("message received: " + msg);
        }
    }
}