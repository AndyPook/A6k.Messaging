using Confluent.Kafka;

namespace A6k.Messaging.Kafka.Features
{
    /// <summary>
    /// Indicates that the Feature requires the Kafka Consumer to do it's thing
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IFeatureRequiresConsumer<TKey, TValue>
    {
        /// <summary>
        /// Capture the Kafka Consumer
        /// </summary>
        /// <param name="consumer"></param>
        void SetConsumer(IConsumer<TKey, TValue> consumer);
    }
}
