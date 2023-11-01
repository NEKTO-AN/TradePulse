using Collector;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<KafkaConsumerWorker>();
        services.AddHostedService<Worker>();
    })
    .ConfigureAppConfiguration((config) =>
    {
        config.AddEnvironmentVariables();
    })
    .Build();

host.Run();

