using Confluent.Kafka;
using Domain.Orderbook;
using IoC.Configuration;
namespace DataConsumer;

public class Worker : BackgroundService
{
        private readonly string _topic;
        private readonly ILogger<Worker> _logger;
        private readonly IConsumer<Null, string> _consumer;
        private readonly IOrderbookRepository _orderbookRepository;

    public Worker(ILogger<Worker> logger, IOrderbookRepository orderbookRepository, OrderbookCollectorConfiguration configuration)
    {
        _logger = logger;
        _orderbookRepository = orderbookRepository;
        _topic = configuration.KafkaOrderbookTopic;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration.KafkaEndpoint,
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
            ConsumeResult<Null, string> result = _consumer.Consume(timeout: TimeSpan.FromMinutes(5));
            if (result is null)
                continue;

            OrderbookResponse? response = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderbookResponse>(result.Message.Value);
            if (response is null || response.Data is null)
                continue;

            //do some logic with it
            _logger.LogInformation(message: response.ToString());

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
