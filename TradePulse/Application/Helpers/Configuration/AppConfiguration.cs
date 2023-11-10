using Application.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Application.Helpers.Configuration
{
    public class AppConfiguration
    {
        public string KafkaOrderbookTopic { get; set; }
        public string KafkaEndpoint { get; set; }

        public string[] ExchangeTopics { get; set; }
        public string[] Symbols { get; set; }

        public AppConfiguration(IConfiguration configuration)
        {
            KafkaOrderbookTopic = configuration["KAFKA_ORDERBOOK_TOPIC"] ?? throw new NotFoundConfigurationException("KAFKA_ORDERBOOK_TOPIC");
            KafkaEndpoint = configuration["KAFKA_ENDPOINT"] ?? throw new NotFoundConfigurationException("KAFKA_ENDPOINT");

            Symbols = configuration.GetSection("Symbols").GetChildren().Select(x => x.Value ?? throw new NotFoundConfigurationException("Symbols")).ToArray();
            ExchangeTopics = Symbols
                .Select(s => configuration
                    .GetSection("Topics")
                    .GetChildren()
                    .Select(x => (x.Value ?? throw new NotFoundConfigurationException("Topics")) + "." + s))
                    .SelectMany(x => x)
                    .ToArray();
        }
    }
}