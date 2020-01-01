using Confluent.Kafka;
using A6k.Messaging.Features;
using System;

namespace A6k.Messaging.Kafka.Features
{
    public interface IKafkaConfigFeature<T> : IFeature where T : ClientConfig
    {
        void Configure(T config);
    }

    public interface IKafkaConfigBuilderFeature<T> : IFeature
    {
        void Configure(T configBuilder);
    }

    public class ConsumerBuilderFeature<Tkey, TValue> : IKafkaConfigBuilderFeature<ConsumerBuilder<Tkey, TValue>>
    {
        private readonly Action<ConsumerBuilder<Tkey, TValue>> config;

        public ConsumerBuilderFeature(Action<ConsumerBuilder<Tkey, TValue>> config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Configure(ConsumerBuilder<Tkey, TValue> configBuilder) => config(configBuilder);
    }

    public class ProducerBuilderFeature<Tkey, TValue> : IKafkaConfigBuilderFeature<ProducerBuilder<Tkey, TValue>>
    {
        private readonly Action<ProducerBuilder<Tkey, TValue>> config;

        public ProducerBuilderFeature(Action<ProducerBuilder<Tkey, TValue>> config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Configure(ProducerBuilder<Tkey, TValue> configBuilder) => config(configBuilder);
    }
}
