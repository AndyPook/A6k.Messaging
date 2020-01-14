using Confluent.Kafka;

namespace A6k.Messaging.Kafka.Features
{
    public interface IKafkaSerializerFactory
    {
        ISerializer<T> Create<T>();
    }
}
