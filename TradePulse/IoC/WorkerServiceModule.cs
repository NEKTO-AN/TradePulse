using Application.Behaviors.Exchange;
using Application.Helpers.Configuration;
using Application.Services.DataConsumerService;
using Domain.Orderbook;
using Domain.PumpDumpSnapshot;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoC
{
    public static class WorkerServiceModule
    {
        public static IServiceCollection AddWorkerServiceModule(this IServiceCollection services)
        {
            services.AddSingleton<MongoDbContext>();
            services.AddSingleton<IOrderbookRepository, OrderbookRepository>();
            services.AddSingleton<IPumpDumpSnapshotRepository, PumpDumpSnapshotRepository>();
            services.AddSingleton<ExchangeWebSocketBehavior>();

            services.AddSingleton<DataConsumerWorkerService>();

            services.AddSingleton<AppConfiguration>((provider) => new(provider.GetService<IConfiguration>() ?? throw new Exception()));

            return services;
        }
    }
}

