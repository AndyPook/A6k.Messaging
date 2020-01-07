using Newtonsoft.Json;

namespace A6k.Messaging.Kafka.Features
{
    public interface IKafkaSerializationFeature
    {
        JsonSerializerSettings KeySettings { get; }
        JsonSerializerSettings ValueSettings { get; }
    }
}
