using System;
using Confluent.Kafka;

namespace A6k.Messaging.Kafka.Features
{
    public class ProducerFactoryFeature
    {
        public static ProducerFactoryFeature<TKey, TValue> Create<TKey, TValue>(Func<ProducerBuilder<TKey, TValue>, IProducer<TKey, TValue>> producerFactory)
        {
            return new ProducerFactoryFeature<TKey, TValue>(producerFactory);
        }
    }

    public class ProducerFactoryFeature<TKey, TValue>
    {
        public ProducerFactoryFeature(Func<ProducerBuilder<TKey, TValue>, IProducer<TKey, TValue>> producerFactory)
        {
            ProducerFactory = producerFactory ?? throw new ArgumentNullException(nameof(producerFactory));
        }

        public Func<ProducerBuilder<TKey, TValue>, IProducer<TKey, TValue>> ProducerFactory { get; }
    }
}
