using System;
using Confluent.Kafka;
using Domain.Orderbook;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Collector
{
	public class KafkaConsumerWorker : BackgroundService
    {
        private const string _groupId = "demo-topic";
        private readonly ILogger<KafkaConsumerWorker> _logger;
        private readonly IConsumer<Null, string> _consumer;

        public KafkaConsumerWorker(ILogger<KafkaConsumerWorker> logger)
        {
            _logger = logger;

            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_ENDPOINT") ?? throw new Exception(),
                GroupId = _groupId + "group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Null, string>(config).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(() =>
        {
            _consumer.Subscribe(_groupId);

            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<Null, string> result = _consumer.Consume(stoppingToken);
                if (result is null)
                    continue;

                OrderbookResponse? response = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderbookResponse>(result.Message.Value);
                if (response is null)
                    continue;

                //do some logic with it
                _logger.LogInformation(response.ToString());
            }
        }, stoppingToken);
    }
}

