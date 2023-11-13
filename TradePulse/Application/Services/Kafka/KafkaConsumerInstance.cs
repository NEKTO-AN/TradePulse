using Confluent.Kafka;

namespace Application.Services.Kafka
{
    public class KafkaConsumerInstance<TKey, TValue> : IDisposable
    {
        private readonly TimeSpan _consumeTimeout = TimeSpan.FromMinutes(5);
        private readonly IConsumer<TKey, TValue> _consumer;
        public KafkaConsumerInstance(IConsumer<TKey, TValue> consumer)
        {
            _consumer = consumer;
        }
        
        public void Subscribe(string topicName)
        {
            _consumer.Subscribe(topicName);
        }

        public async Task ConsumeMessageUntilCancel(Func<TValue, Task> func, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ConsumeMessage(func);
            }
        }


        public Task ConsumeMessage(Func<TValue, Task> func)
        {
            ConsumeResult<TKey, TValue> consumeResult = _consumer.Consume(timeout: _consumeTimeout);
            if (consumeResult.Message.Value == null)
            {
                return Task.CompletedTask;
            }

            return func(consumeResult.Message.Value);
        }

        public void Stop()
        {
            _consumer.Commit();

            _consumer.Unsubscribe();
        }

        public void Dispose() => _consumer.Dispose();
    }
}