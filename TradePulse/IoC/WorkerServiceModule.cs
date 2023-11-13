using System.Net.WebSockets;
using Application.Abstractions;
using Application.Behaviors.Exchange;
using Application.Behaviors.Socket;
using Application.Helpers.Configuration;
using Application.Services.DataConsumerService;
using Application.Services.Kafka;
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
            services.AddSingleton<IWebSocketHandler, WebSocketHandler>((provider) => new WebSocketHandler(new ClientWebSocket()));

            services.AddSingleton<DataConsumerWorkerService>();
            services.AddSingleton<KafkaAdmin>();
            services.AddSingleton<KafkaProducer>();
            services.AddSingleton<KafkaConsumer>();

            services.AddSingleton<AppConfiguration>((provider) => new(provider.GetService<IConfiguration>() ?? throw new Exception()));

            return services;
        }
    }
}

