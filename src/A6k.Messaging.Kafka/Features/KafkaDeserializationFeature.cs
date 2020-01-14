using Confluent.Kafka;

namespace A6k.Messaging.Kafka.Features
{
    public class KafkaDeserializationFeature<TKey, TValue>
    {
        public IDeserializer<TKey> KeyDeserializer { get; set; }
        public IDeserializer<TValue> ValueDeserializer { get; set; }
    }
}
