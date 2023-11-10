using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Application.Helpers.Kafka
{
    public class MessageSerializer<T> : ISerializer<T>, IDeserializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context) 
            => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Formatting.None));

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
        }
    }
}