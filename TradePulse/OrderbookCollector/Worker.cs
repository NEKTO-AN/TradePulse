using Application.Behaviors.Exchange;
using Application.Helpers.Configuration;
using Application.Helpers.Kafka;
using Application.Services.Kafka;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Domain.Exchange.WebSocket;
using Domain.Orderbook;

namespace OrderbookCollector;

public class Worker : BackgroundService
{
    private readonly string[] _orderbookTopics;
    private readonly ILogger<Worker> _logger;
    private readonly KafkaProducerInstance<Null, OrderbookDataMessage> _producer;
    private readonly ExchangeWebSocketBehavior _exchangeWebSocketBehavior;

    private readonly Dictionary<string, double> _lastSymbolPrice;

    public Worker(ILogger<Worker> logger, ExchangeWebSocketBehavior exchangeWebSocketBehavior, KafkaAdmin kafkaAdmin, KafkaProducer kafkaProducer, AppConfiguration configuration)
    {
        _logger = logger;
        _orderbookTopics = configuration.ExchangeTopics;
        _exchangeWebSocketBehavior = exchangeWebSocketBehavior;

        kafkaAdmin.CreateTopic(configuration.KafkaOrderbookTopic, 120000);  // 2 minutes
        _producer = kafkaProducer.BuildInstance<Null, OrderbookDataMessage>(configuration.KafkaOrderbookTopic);

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
        if (IsNotValidOrderbookDataMessage(orderbookDataMessage))
            return;

        orderbookDataMessage!.LastPrice = _lastSymbolPrice[orderbookDataMessage.Data!.Symbol];

        await _producer.ProduceAsync(orderbookDataMessage);

        _logger.LogInformation(value);
    }

    private bool IsNotValidOrderbookDataMessage(OrderbookDataMessage? orderbookDataMessage) 
        => orderbookDataMessage?.Data == null || _lastSymbolPrice[orderbookDataMessage.Data.Symbol] == -1
            || (orderbookDataMessage.Data.Bids.Length < 1 && orderbookDataMessage.Data.Asks.Length < 1);
}

