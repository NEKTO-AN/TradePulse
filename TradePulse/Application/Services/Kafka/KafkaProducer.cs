using Application.Helpers.Configuration;
using Application.Helpers.Kafka;
using Confluent.Kafka;

namespace Application.Services.Kafka
{
    public class KafkaProducer
    {
        private readonly ProducerConfig producerConfig;

        public KafkaProducer(AppConfiguration configuration)
        {
            producerConfig = new()
            {
                BootstrapServers = configuration.KafkaEndpoint
            };
        }

        public KafkaProducerInstance<TKey, TValue> BuildInstance<TKey, TValue>(string topicName)
        {
            IProducer<TKey, TValue> producer = new ProducerBuilder<TKey, TValue>(producerConfig)
                .SetValueSerializer(new MessageSerializer<TValue>())
                .Build();
                
            return new KafkaProducerInstance<TKey, TValue>(topicName, producer);
        }
    }
}