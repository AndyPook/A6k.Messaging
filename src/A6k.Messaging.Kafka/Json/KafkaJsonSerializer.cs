using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace A6k.Messaging.Kafka.Json
{
    public class KafkaJsonSerializer<T> : ISerializer<T>
        {
            private readonly JsonSerializerSettings settings;

            public KafkaJsonSerializer(JsonSerializerSettings settings = null)
            {
                this.settings = settings;
            }

            public byte[] Serialize(T data, SerializationContext context)
            {
                //TODO optimize
                return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, settings));
            }
        }
}
