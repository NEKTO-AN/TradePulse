using Collector;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<KafkaConsumerWorker>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();

