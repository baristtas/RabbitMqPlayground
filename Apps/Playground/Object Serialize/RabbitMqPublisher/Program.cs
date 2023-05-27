using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

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

            channel.QueueDeclare("products", true, false, false, null);


            var product = new Product { id = 1, name = "ram", price = 100, stock = 5 };
            var serializedProductStr = JsonSerializer.Serialize(product);

            var msgBody = Encoding.UTF8.GetBytes(serializedProductStr);

            channel.BasicPublish(string.Empty, "products", null, msgBody);

            Console.WriteLine("Msg sent.");

            Console.ReadLine();
        }
    }
}