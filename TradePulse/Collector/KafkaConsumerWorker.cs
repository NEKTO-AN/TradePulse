using Confluent.Kafka;
using Domain.Orderbook;

namespace Collector
{
    public class KafkaConsumerWorker : BackgroundService
    {
        private readonly string _topic;
        private readonly ILogger<KafkaConsumerWorker> _logger;
        private readonly IConsumer<Null, string> _consumer;
        private readonly IOrderbookRepository _orderbookRepository;

        public KafkaConsumerWorker(ILogger<KafkaConsumerWorker> logger, IOrderbookRepository orderbookRepository, IConfiguration configuration)
        {
            _logger = logger;
            _orderbookRepository = orderbookRepository;
            _topic = configuration["KAFKA_ORDERBOOK_TOPIC"] ?? throw new Exception();

            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_ENDPOINT") ?? throw new Exception(),
                GroupId = _topic + "group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Null, string>(config).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(() =>
        {
            _consumer.Subscribe(_topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Null, string> result = _consumer.Consume(stoppingToken);
                if (result is null)
                    continue;

                OrderbookResponse? response = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderbookResponse>(result.Message.Value);
                if (response is null || response.Data is null)
                    continue;

                //do some logic with it
                _logger.LogInformation(response.ToString());

                _orderbookRepository.AddAsync(new Orderbook(
                    timestamp: response.Timestamp,
                    symbol: response.Data.Symbol,
                    data: new(
                        response.Data.Bids,
                        response.Data.Asks,
                        response.Data.CrossSequence)));
            }
        }, stoppingToken);
    }
}

