using Application.Behaviors.Exchange;
using Confluent.Kafka;

namespace Collector;

public class Worker : BackgroundService
{
    private readonly string[] topics = new string[] { "orderbook.500.ETHUSDT", "orderbook.500.BTCUSDT" };
    private readonly string _topic;
    private readonly ILogger<Worker> _logger;
    private readonly IProducer<Null, string> _producer;
    private readonly ExchangeWebSocketBehavior _exchangeWebSocketBehavior;

    public Worker(ExchangeWebSocketBehavior exchangeWebSocketBehavior, ILogger<Worker> logger, IConfiguration configuration)
    {
        _exchangeWebSocketBehavior = exchangeWebSocketBehavior;
        _logger = logger;
        _topic = configuration["KAFKA_ORDERBOOK_TOPIC"] ?? throw new Exception();

        ProducerConfig config = new()
        {
            BootstrapServers = configuration["KAFKA_ENDPOINT"] ?? throw new Exception()
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => _exchangeWebSocketBehavior.OrderbookAsync(topics, MessageAsync, stoppingToken);

    private Task MessageAsync(string value)
        => _producer.ProduceAsync(_topic, new Message<Null, string>
        {
            Value = value
        });
}
