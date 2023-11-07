using Microsoft.Extensions.Configuration;

namespace IoC.Configuration
{
    public class OrderbookCollectorConfiguration
    {
        public string KafkaOrderbookTopic { get; set; }
        public string KafkaEndpoint { get; set; }

        public string[] OrderbookTopics { get; set; }

        public OrderbookCollectorConfiguration(IConfiguration configuration)
        {
            KafkaOrderbookTopic = configuration["KAFKA_ORDERBOOK_TOPIC"] ?? throw new Exception();
            KafkaEndpoint = configuration["KAFKA_ENDPOINT"] ?? throw new Exception();

            OrderbookTopics = configuration.GetSection("Topics").GetChildren().Select(x => x.Value ?? throw new Exception()).ToArray();
        }
	}
}

