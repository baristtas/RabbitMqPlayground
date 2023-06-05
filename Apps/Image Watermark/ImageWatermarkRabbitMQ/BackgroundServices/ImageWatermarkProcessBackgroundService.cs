using ImageWatermarkRabbitMQ.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Drawing;
using System.Text.Json;

namespace ImageWatermarkRabbitMQ.BackgroundServices
{
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService m_rabbitMqClientService;
        private readonly ILogger<ImageWatermarkProcessBackgroundService> m_logger;
        private RabbitMQ.Client.IModel m_channel;

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            m_channel = m_rabbitMqClientService.connect();
            m_channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(m_channel);

            m_channel.BasicConsume(RabbitMQClientService.queueName, false, consumer);

            consumer.Received += Consumer_Received;

            return Task.CompletedTask;

        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));
            var path = Path.Combine(Directory.GetCurrentDirectory() + "wwwroot/Images" + productImageCreatedEvent.imageName);

            using var img = Image.FromFile(path);

            using var graphic = Graphics.FromImage(img);

            var font = new Font(FontFamily.GenericSansSerif,10);

            var textSize = graphic.MeasureString("testbaristest",font);

            var color = Color.FromArgb(255,255,255,255);
            var brush = new SolidBrush(color);

            var position = new Point(img.Width-((int)textSize.Width+30),(int)textSize.Height-30);

            graphic.DrawString("testbaristest",font,brush,position);

            graphic.Save(); 

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
