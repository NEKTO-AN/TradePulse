using Application.Helpers.Configuration;
using Application.Services.DataConsumerService;
using Application.Services.Kafka;
using Confluent.Kafka;
using Domain.Orderbook;
namespace DataConsumer;

public class Worker : BackgroundService
{
    private readonly string _topic;
    private readonly ILogger<Worker> _logger;
    private readonly KafkaConsumerInstance<Null, OrderbookDataMessage> _consumer;
    private readonly DataConsumerWorkerService _dataConsumerWorkerService;

    public Worker(ILogger<Worker> logger, DataConsumerWorkerService dataConsumerWorkerService, KafkaConsumer kafkaConsumer, AppConfiguration configuration)
    {
        _logger = logger;
        _topic = configuration.KafkaOrderbookTopic;
        _dataConsumerWorkerService = dataConsumerWorkerService;
        
        _consumer = kafkaConsumer.ConsumeTopic<Null, OrderbookDataMessage>(configuration.KafkaOrderbookTopic);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);

        return _consumer.ConsumeMessageUntilCancel(async (message) =>
        {
            if (message.Data == null || (message.Data.Bids.Length < 1 && message.Data.Asks.Length < 1))
                return;
                
            _logger.LogInformation("ConsumeMessageUntilCancel {0}", message);

            await _dataConsumerWorkerService.AddOrderbookItemAsync(new Orderbook(message.Timestamp, message.Data));
            await _dataConsumerWorkerService.FindAnomalyAsync(message.Data.Symbol, message.Timestamp, message.LastPrice);
        }, stoppingToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Commit kafka data");
        _consumer.Stop();

        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}
