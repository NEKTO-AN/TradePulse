using Application.Behaviors.Exchange;
using Confluent.Kafka;
using IoC.Configuration;

namespace OrderbookCollector;

public class Worker : BackgroundService
{
    private readonly string[] _orderbookTopics;
    private readonly string _topic;
    private readonly IProducer<Null, string> _producer;
    private readonly ExchangeWebSocketBehavior _exchangeWebSocketBehavior;

    public Worker(ExchangeWebSocketBehavior exchangeWebSocketBehavior, OrderbookCollectorConfiguration configuration)
    {
        _topic = configuration.KafkaOrderbookTopic;
        _orderbookTopics = configuration.OrderbookTopics;
        _exchangeWebSocketBehavior = exchangeWebSocketBehavior;

        ProducerConfig config = new()
        {
            BootstrapServers = configuration.KafkaEndpoint
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => _exchangeWebSocketBehavior.OrderbookAsync(_orderbookTopics, MessageAsync, stoppingToken);

    private Task MessageAsync(string value)
        => _producer.ProduceAsync(_topic, new Message<Null, string>
        {
            Value = value
        });
}

