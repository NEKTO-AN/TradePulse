using Application.Behaviors.Exchange;
using Application.Helpers.Configuration;
using Application.Helpers.Kafka;
using Confluent.Kafka;
using Domain.Exchange.WebSocket;
using Domain.Orderbook;

namespace OrderbookCollector;

public class Worker : BackgroundService
{
    private readonly string[] _orderbookTopics;
    private readonly ILogger<Worker> _logger;
    private readonly string _topic;
    private readonly IProducer<Null, OrderbookDataMessage> _producer;
    private readonly ExchangeWebSocketBehavior _exchangeWebSocketBehavior;

    private readonly Dictionary<string, double> _lastSymbolPrice;

    public Worker(ILogger<Worker> logger, ExchangeWebSocketBehavior exchangeWebSocketBehavior, AppConfiguration configuration)
    {
        _logger = logger;
        _topic = configuration.KafkaOrderbookTopic;
        _orderbookTopics = configuration.ExchangeTopics;
        _exchangeWebSocketBehavior = exchangeWebSocketBehavior;

        ProducerConfig config = new()
        {
            BootstrapServers = configuration.KafkaEndpoint
        };

        _producer = new ProducerBuilder<Null, OrderbookDataMessage>(config)
            .SetValueSerializer(new MessageSerializer<OrderbookDataMessage>()).Build();

        _lastSymbolPrice = configuration.Symbols.ToDictionary(k => k, v => -1.0);
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => _exchangeWebSocketBehavior.OrderbookAsync(_orderbookTopics, MessageAsync, stoppingToken);

    private async Task MessageAsync(string value)
    {
        if (value.IndexOf("tickers") > 0)
        {
            TickerResponse? tickerResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<TickerResponse>(value);
            if (tickerResponse != null && tickerResponse.Data.LastPrice > 0)
            {
                _lastSymbolPrice[tickerResponse.Data.Symbol] = tickerResponse.Data.LastPrice;
            }

            return;
        }

        OrderbookDataMessage? orderbookDataMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderbookDataMessage>(value);
        if (orderbookDataMessage?.Data == null 
            || _lastSymbolPrice[orderbookDataMessage.Data.Symbol] == -1)
            return;

        orderbookDataMessage.LastPrice = _lastSymbolPrice[orderbookDataMessage.Data.Symbol];

        await _producer.ProduceAsync(_topic, new Message<Null, OrderbookDataMessage>
        {
            Value = orderbookDataMessage
        });

        _logger.LogInformation(value);
    }
}

