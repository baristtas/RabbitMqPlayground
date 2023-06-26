using CreateExcelFile.Models;
using ExcelFileCreateWorkerService.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using System;
using WorkerService;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

IHost host = Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{

    StreamReader sr = new StreamReader("C:\\Users\\baris.tas\\Desktop\\rbmq\\amqpinstanceuri.txt");
    var uri = sr.ReadToEnd();
    services.AddSingleton<RabbitMQClientService>();
    services.AddSingleton(sp => new ConnectionFactory()
    {

        Uri = new Uri(uri),
        DispatchConsumersAsync = true
    });

    services.AddDbContext<AdventureWorks2019Context>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
    });
    services.AddHostedService<Worker>();

})
.Build();

await host.RunAsync();
