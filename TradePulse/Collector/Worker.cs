using System.Net.WebSockets;
using Application.Behaviors.Socket;
using Confluent.Kafka;
using Domain.Orderbook;

namespace Collector;

public class Worker : BackgroundService
{
    private const string _symbol = "OPUSDT";
    private const string _groupId = "demo-topic";

    private readonly ILogger<Worker> _logger;
    private readonly ClientWebSocket ws = new();

    private readonly IProducer<Null, string> _producer;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;

        ProducerConfig config = new()
        {
            BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_ENDPOINT") ?? throw new Exception()
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        WebSocketBehavior webSocket = new(
            handler: new WebSocketHandler(ws),
            url: "wss://stream.bybit.com/v5/public/linear",
            receiveBufferSize: 1024 * 32);

        await webSocket.ConnectAsync(stoppingToken);
        await webSocket.SendAsync("{\"op\":\"subscribe\",\"args\":[\"orderbook.500." + _symbol + "\"]}", stoppingToken);


        webSocket.OnMessageReceived(MessageAsync, stoppingToken);
    }

    private Task MessageAsync(string value)
    {
        OrderbookResponse? response = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderbookResponse>(value);
        if (response is null)
        {
            return Task.CompletedTask;
        }

        _logger.LogInformation(message: response.ToString());

        return _producer.ProduceAsync(_groupId, new Message<Null, string>
        {
            Value = value
        });
    }
}
