using ImageWatermarkRabbitMQ.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Drawing;
using System.Text.Json;

namespace ImageWatermarkRabbitMQ.BackgroundServices
{
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService m_rabbitMqClientService;
        private readonly ILogger<ImageWatermarkProcessBackgroundService> m_logger;
        private RabbitMQ.Client.IModel m_channel;

        public ImageWatermarkProcessBackgroundService(RabbitMQClientService rabbitMqClientService, ILogger<ImageWatermarkProcessBackgroundService> logger)
        {
            m_rabbitMqClientService = rabbitMqClientService;
            m_logger = logger;
        }

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
            try
            {


                var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(System.Text.Encoding.UTF8.GetString(@event.Body.ToArray()));
                var path = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/Images/" + productImageCreatedEvent.imageName);

                using Image img = Image.FromFile(path);

                using var graphic = Graphics.FromImage(img);

                var font = new Font(FontFamily.GenericSansSerif, 35);

                var textSize = graphic.MeasureString("testbaristest", font);

                var color = Color.FromArgb(128, 0, 0, 0);
                var brush = new SolidBrush(color);

                var position = new Point((img.Width/2) - ((int)textSize.Width/2),(img.Height/2) - ((int)textSize.Height/2));

                graphic.DrawString("testbaristest", font, brush, position);


                img.Save("wwwroot/Images/Watermarks/" + productImageCreatedEvent.imageName);

                img.Dispose();
                graphic.Dispose();
                m_channel.BasicAck(@event.DeliveryTag, false);
            }
            catch(Exception ex)
            {
                
            }

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
