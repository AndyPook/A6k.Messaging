using System.Threading.Tasks;

namespace A6k.Messaging
{
    /// <summary>
    /// Extensions to make sending messages easier
    /// </summary>
    public static class IMessageProducerExtensions
    {
        /// <summary>
        /// Send a message with key/value rather than message
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="producer"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task ProduceAsync<TKey, TValue>(this IMessageProducer<TKey, TValue> producer, TKey key, TValue value)
        {
            var message = new Message<TKey, TValue>
            {
                Key = key,
                Value = value
            };

            return producer.ProduceAsync(message);
        }
    }
}
