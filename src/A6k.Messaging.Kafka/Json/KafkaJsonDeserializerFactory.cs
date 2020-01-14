using Confluent.Kafka;

using A6k.Messaging.Kafka.Features;

namespace A6k.Messaging.Kafka.Json
{
    public class KafkaJsonDeserializerFactory : IKafkaDeserializerFactory
    {
        public IDeserializer<T> Create<T>() => new KafkaJsonDeserializer<T>();
    }
}