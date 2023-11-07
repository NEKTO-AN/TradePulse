using Collector;
using IoC;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<KafkaConsumerWorker>()
            .AddWorkerServiceModule();
        services.AddHostedService<Worker>()
            .AddWorkerServiceModule();
    })
    .ConfigureAppConfiguration((config) =>
    {
        config.AddEnvironmentVariables();
    })
    .Build();

host.Run();

