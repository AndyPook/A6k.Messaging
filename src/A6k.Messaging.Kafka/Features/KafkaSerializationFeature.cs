using Newtonsoft.Json;

namespace A6k.Messaging.Kafka.Features
{
    public class KafkaSerializationFeature : IKafkaSerializationFeature
    {
        public KafkaSerializationFeature(JsonSerializerSettings valueSettings)
        {
            ValueSettings = valueSettings;
        }

        public KafkaSerializationFeature(JsonSerializerSettings keySettings, JsonSerializerSettings valueSettings)
        {
            KeySettings = keySettings;
            ValueSettings = valueSettings;
        }

        public JsonSerializerSettings KeySettings { get; }
        public JsonSerializerSettings ValueSettings { get; }
    }
}
