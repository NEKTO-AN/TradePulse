using Application;
using Domain.Orderbook;
using Domain.PumpDumpSnapshot;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace IoC
{
    public static class WebApiModule
    {
        public static IServiceCollection AddWebApiModule(this IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssembly(AssemblyReference.Assembly));

            services.AddScoped<IOrderbookRepository, OrderbookRepository>();
            services.AddScoped<IPumpDumpSnapshotRepository, PumpDumpSnapshotRepository>();
            services.AddScoped<MongoDbContext>();

            return services;
        }
    }
}

