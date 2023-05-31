using ImageWatermarkRabbitMQ.Models;
using ImageWatermarkRabbitMQ.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace ImageWatermarkRabbitMQ
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //RabbitMQ cloud uri
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

            // Add services to the container.
            builder.Services.AddSingleton(sp => new ConnectionFactory() {
                Uri = new Uri(uri) 
            });
            builder.Services.AddSingleton<RabbitMQClientService>();


            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: "productDb");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}