using System;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace A6k.Messaging.Kafka.Json
{
    public class KafkaJsonDeserializer<T> : IDeserializer<T>
    {
        private readonly JsonSerializerSettings settings;

        public KafkaJsonDeserializer(JsonSerializerSettings settings = null)
        {
            this.settings = settings;
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            //TODO optimize
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data.ToArray()), settings);
        }
    }
}