using A6k.Messaging.Features;
using Newtonsoft.Json;

namespace A6k.Messaging.Kafka.Features
{
    public interface IKafkaSerializationFeature : IFeature
    {
        JsonSerializerSettings KeySettings { get; }
        JsonSerializerSettings ValueSettings { get; }
    }
}
