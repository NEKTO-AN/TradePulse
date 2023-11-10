using Application.Helpers.Configuration;
using Application.Helpers.Kafka;
using Application.Services.DataConsumerService;
using Confluent.Kafka;
using Domain.Orderbook;
namespace DataConsumer;

public class Worker : BackgroundService
{
    private readonly string _topic;
    private readonly ILogger<Worker> _logger;
    private readonly IConsumer<Null, OrderbookDataMessage> _consumer;
    private readonly DataConsumerWorkerService _dataConsumerWorkerService;

    public Worker(ILogger<Worker> logger, DataConsumerWorkerService dataConsumerWorkerService, AppConfiguration configuration)
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

        _consumer = new ConsumerBuilder<Null, OrderbookDataMessage>(config)
            .SetValueDeserializer(new MessageSerializer<OrderbookDataMessage>()).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
    {
        _consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            ConsumeResult<Null, OrderbookDataMessage> consumeResult = _consumer.Consume(timeout: TimeSpan.FromMinutes(5));
            _logger.LogInformation(message: consumeResult.Message.Value.ToString());

            if (consumeResult?.Message?.Value?.Data is null || (consumeResult.Message.Value.Data.Bids.Length < 1 && consumeResult.Message.Value.Data.Asks.Length < 1))
                continue;



            await _dataConsumerWorkerService.AddOrderbookItemAsync(
                lastPrice: consumeResult.Message.Value.LastPrice,
                orderbook: new Orderbook(consumeResult.Message.Value.Timestamp, consumeResult.Message.Value.Data));

            _consumer.Commit(consumeResult);
        }
    }, stoppingToken);
}
