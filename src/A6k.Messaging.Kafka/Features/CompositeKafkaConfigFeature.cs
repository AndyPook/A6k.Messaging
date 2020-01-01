using Confluent.Kafka;
using A6k.Messaging.Features;

namespace A6k.Messaging.Kafka.Features
{
    public class CompositeKafkaConfigFeature<T> : CompositeFeature<IKafkaConfigFeature<T>>, IKafkaConfigFeature<T>
        where T : ClientConfig
    {
        public void Configure(T config) => Invoke(f => f.Configure(config));
    }
}
