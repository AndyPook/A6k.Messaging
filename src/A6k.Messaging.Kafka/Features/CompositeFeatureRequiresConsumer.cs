using Confluent.Kafka;
using A6k.Messaging.Features;

namespace A6k.Messaging.Kafka.Features
{
    /// <inheritdoc />
    public class CompositeFeatureRequiresConsumer<TKey, TValue> : CompositeFeature<IFeatureRequiresConsumer<TKey, TValue>>, IFeatureRequiresConsumer<TKey, TValue>
    {
        /// <inheritdoc />
        public void SetConsumer(IConsumer<TKey, TValue> consumer) => Invoke(f => f.SetConsumer(consumer));
    }
}
