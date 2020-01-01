using System;
using System.Collections.Generic;
using System.Threading;
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
        
        private KeyValueListNode headers;

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
        public TKey Key { get; set; }

        /// <inheritdoc/>
        public TValue Value { get; set; }

        public IEnumerable<KeyValuePair<string, object>> Headers
        {
            get
            {
                if (headers == null)
                    yield break;

                for (var h = headers; h != null; h = h.Next)
                    yield return h.KeyValue;
            }
        }

        public object GetHeader(string key)
        {
            foreach (var keyValue in Headers)
                if (key == keyValue.Key)
                    return keyValue.Value;

            return null;
        }

        public IMessage AddHeader(string key, object value)
        {
            var currentHeader = headers;
            var newHeaders = new KeyValueListNode { KeyValue = new KeyValuePair<string, object>(key, value) };

            do
            {
                newHeaders.Next = currentHeader;
                currentHeader = Interlocked.CompareExchange(ref headers, newHeaders, currentHeader);
            } while (!ReferenceEquals(newHeaders.Next, currentHeader));

            return this;
        }

        public void ForEachHeader(Action<KeyValuePair<string, object>> action)
        {
            if (headers == null)
                return;
            foreach (var h in Headers)
                action(h);
        }

        private partial class KeyValueListNode
        {
            public KeyValuePair<string, object> KeyValue;
            public KeyValueListNode Next;
        }
    }
}
