using System;
using System.Threading.Tasks;

namespace A6k.Messaging
{
    /// <summary>
    /// Implementation of <see cref="IMessage{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="TKey">Typeof Key</typeparam>
    /// <typeparam name="TValue">Typeof Value</typeparam>
    public class Message<TKey, TValue> : IMessage<TKey, TValue>
    {
        public static readonly Task<IMessage<TKey, TValue>> Null = Task.FromResult<IMessage<TKey, TValue>>(null);

        /// <inheritdoc/>
        public string ActivityId { get; set; }

        /// <inheritdoc/>
        public DateTime Timestamp { get; set; }

        /// <inheritdoc/>
        public string Topic { get; set; }

        /// <inheritdoc/>
        public int Partition { get; set; }

        /// <inheritdoc/>
        public long Offset { get; set; }

        /// <inheritdoc/>
        /// <inheritdoc/>
        IMessageHeaders IMessage.Headers => Headers;
        public MessageHeaders Headers { get; private set; }

        public Message<TKey, TValue> AddHeader(string key, object value) {
            if (Headers == null)
                Headers = new MessageHeaders();
            Headers.AddHeader(key, value);
            return this;
        }

        /// <inheritdoc/>
        public TKey Key { get; set; }

        /// <inheritdoc/>
        public TValue Value { get; set; }
    }
}
