using Confluent.Kafka;

namespace A6k.Messaging.Kafka.Features
{
    public interface IKafkaDeserializerFactory
    {
        IDeserializer<T> Create<T>();
    }
}
