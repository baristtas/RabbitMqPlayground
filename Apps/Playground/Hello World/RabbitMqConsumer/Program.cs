﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

            var factory = new ConnectionFactory();
            factory.Uri = new Uri(uri);
            
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