using Collector;
using Domain.Orderbook;
using Infrastructure;
using Infrastructure.Repositories;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<KafkaConsumerWorker>()
            .AddSingleton<MongoDbContext>()
            .AddSingleton<IOrderbookRepository, OrderbookRepository>();
        services.AddHostedService<Worker>();
    })
    .ConfigureAppConfiguration((config) =>
    {
        config.AddEnvironmentVariables();
    })
    .Build();

host.Run();

