using Confluent.Kafka;

namespace Application.Services.Kafka
{
    public class KafkaProducerInstance<TKey, TValue>
    {
        private readonly string _topic;
        private readonly IProducer<TKey, TValue> _producer;
        public KafkaProducerInstance(string topic, IProducer<TKey, TValue> producer)
        {
            _topic = topic;
            _producer = producer;
        }

        public Task<DeliveryResult<TKey, TValue>> ProduceAsync(TValue message) => _producer.ProduceAsync(_topic, new Message<TKey, TValue>
        {
            Value = message
        });
    }
}