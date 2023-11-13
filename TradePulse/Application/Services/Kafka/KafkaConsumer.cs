using Application.Helpers.Configuration;
using Application.Helpers.Kafka;
using Confluent.Kafka;

namespace Application.Services.Kafka
{
    public class KafkaConsumer
    {
        private readonly ConsumerConfig consumerConfig;

        public KafkaConsumer(AppConfiguration appConfiguration)
        {
            consumerConfig = new()
            {
                BootstrapServers = appConfiguration.KafkaEndpoint,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
        }

        public KafkaConsumerInstance<TKey, TValue> ConsumeTopic<TKey, TValue>(string topicName)
        {
            consumerConfig.GroupId = topicName + "group";

            IConsumer<TKey, TValue> consumer = new ConsumerBuilder<TKey, TValue>(consumerConfig)
                .SetValueDeserializer(new MessageSerializer<TValue>())
                .Build();

            return new KafkaConsumerInstance<TKey, TValue>(consumer);
        }
    }
}