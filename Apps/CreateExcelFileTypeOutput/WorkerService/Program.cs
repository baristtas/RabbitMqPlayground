using RabbitMQ.Client;
using System;
using WorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {

        StreamReader sr = new StreamReader("C:\\Users\\baris.tas\\Desktop\\rbmq\\amqpinstanceuri.txt");
        var uri = sr.ReadToEnd();
        services.AddSingleton(sp => new ConnectionFactory()
        {
            Uri = new Uri(uri),
            DispatchConsumersAsync = true
        });

        services.AddHostedService<Worker>();

    })
    .Build();

await host.RunAsync();
