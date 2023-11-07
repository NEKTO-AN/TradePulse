using Application.Behaviors.Exchange;
using Domain.Orderbook;
using Infrastructure;
using Infrastructure.Repositories;
using IoC.Configuration;
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
            services.AddSingleton<ExchangeWebSocketBehavior>();

            services.AddSingleton<OrderbookCollectorConfiguration>((provider) => new(provider.GetService<IConfiguration>() ?? throw new Exception()));

            return services;
        }
    }
}

