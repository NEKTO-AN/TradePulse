using Application.Services.DataConsumerService;
using Confluent.Kafka;
using Domain.Orderbook;
using IoC.Configuration;
namespace DataConsumer;

public class Worker : BackgroundService
{
    private readonly string _topic;
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<Null, string> _consumer;
    private readonly DataConsumerWorkerService _dataConsumerWorkerService;

    public Worker(ILogger<Worker> logger, DataConsumerWorkerService dataConsumerWorkerService, OrderbookCollectorConfiguration configuration)
    {
        _logger = logger;
        _topic = configuration.KafkaOrderbookTopic;
        _dataConsumerWorkerService = dataConsumerWorkerService;

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration.KafkaEndpoint,
            GroupId = _topic + "group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Null, string>(config).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
    {
        _consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<Null, string> result = _consumer.Consume(timeout: TimeSpan.FromMinutes(5));
            if (result is null)
                continue;

            OrderbookResponse? response = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderbookResponse>(result.Message.Value);
            if (response is null || response.Data is null || (response.Data.Bids.Length < 1 && response.Data.Asks.Length < 1))
                continue;

            _logger.LogInformation(message: response.ToString());

            await _dataConsumerWorkerService.AddOrderbookItemAsync(new Orderbook(response.Timestamp, response.Data));
        }
    }, stoppingToken);
}
