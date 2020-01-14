using Confluent.Kafka;

using A6k.Messaging.Kafka.Features;

namespace A6k.Messaging.Kafka.Json
{
    public class KafkaJsonSerializerFactory : IKafkaSerializerFactory
    {
        public ISerializer<T> Create<T>() => new KafkaJsonSerializer<T>();
    }
}
