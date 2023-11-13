using DataConsumer;
using IoC;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>()
            .AddWorkerServiceModule();
    })
    .ConfigureAppConfiguration(config => 
    {
        config.AddEnvironmentVariables();
    })
    .Build();

host.Run();
