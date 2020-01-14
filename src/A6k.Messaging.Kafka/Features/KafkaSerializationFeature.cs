using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace A6k.Messaging.Kafka.Features
{
    public class KafkaSerializationFeature<TKey, TValue>
    {
        public ISerializer<TKey> KeySerializer { get; set; }
        public ISerializer<TValue> ValueSerializer { get; set; }
    }
}
