using Application.Helpers.Configuration;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Application.Services.Kafka
{
    public class KafkaAdmin
    {
        private readonly AdminClientConfig adminClientConfig = new();

        public KafkaAdmin(AppConfiguration configuration)
        {
            adminClientConfig.BootstrapServers = configuration.KafkaEndpoint;
        }

        public void CreateTopic(string topicName, int messageRetention, int numPartitions = -1, short replicationFactor = -1)
        {
            using IAdminClient adminClient = new AdminClientBuilder(adminClientConfig)
                .Build();
                
            TopicSpecification topicSpecification = new()
            {
                Name = topicName,
                NumPartitions = numPartitions, // You can adjust the number of partitions as needed
                ReplicationFactor = replicationFactor, // Adjust the replication factor as needed
                Configs = new Dictionary<string, string>
                {
                    { "retention.ms", $"{messageRetention}" }
                }
            };

            adminClient.CreateTopicsAsync(new List<TopicSpecification> { topicSpecification }).Wait();
        }
    }
}